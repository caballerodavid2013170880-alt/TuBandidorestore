using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Service.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class VehiculoService : IVehiculoService
  {
    private readonly SuvanDbContext context;
    private readonly ITipoVehiculoService tipoVehiculoService;

    public VehiculoService(SuvanDbContext context, ITipoVehiculoService tipoVehiculoService)
    {
      this.context = context;
      this.tipoVehiculoService = tipoVehiculoService;
    }


    /// <summary>
    /// Obtiene todos los vehículos desde la base de datos, incluyendo la información del tipo de vehículo asociado.
    /// </summary>
    /// <returns>Lista de todos los vehículos.</returns>
    public async Task<List<Vehiculo>> GetAllVehiculos(int empresaId)
    {

      var vehiculos = await context.Vehiculos
        .Include(x => x.TipovehiculoIdtipovehiculoNavigation)
        .Where(x => x.EmpresaIdempresa == empresaId)
        .ToListAsync();
      return vehiculos!;
    }


    /// <summary>
    /// Obtiene el ViewModel para agregar/editar un vehículo.
    /// </summary>
    /// <param name="id">Identificador del vehículo.</param>
    /// <returns>ViewModel para agregar/editar un vehículo.</returns>
    public async Task<AgregarUnidadViewModel> GetVehiculoViewModel(int id, int empresaId)
    {
      Vehiculo? vehiculo = new();
      List<Tipovehiculo> tipoVehiculo = await tipoVehiculoService.GetTipovehiculosActivo();

      if (id > 0)
      {
        vehiculo = await context.Vehiculos
       .FirstOrDefaultAsync(x => x.IdVehiculo == id);
      }

      AgregarUnidadViewModel viewModel = new();

      viewModel.TipoUnidades = tipoVehiculo.Select(x => new TipoUnidadViewModel
      {
        TipoUnidadId = x.Idtipovehiculo,
        Nombre = x.Nombre!
      }).ToList();

      if (vehiculo != null)
      {
        viewModel.UnidadId = id;
        viewModel.Placas = vehiculo?.Placas ?? string.Empty;
        viewModel.Vin = vehiculo?.Vin ?? string.Empty;
        viewModel.TipoUnidadId = vehiculo?.TipovehiculoIdtipovehiculo ?? 0;
        viewModel.Activo = id == 0 ? true : vehiculo?.Activo == 1;
        viewModel.NumeroPoliza = vehiculo?.Numeropoliza ?? string.Empty;
        viewModel.Marca = vehiculo?.Marca ?? string.Empty;
        viewModel.Modelo = vehiculo?.Modelo ?? string.Empty;
        viewModel.NumeroEconomico = vehiculo?.Numeroeconomico ?? string.Empty;
        viewModel.NumeroMotor = vehiculo?.Numeromotor ?? string.Empty;
        viewModel.FechaFinSeguro = vehiculo?.Fechafinseguro;

        var servicios = await context.Vehiculoservicios
          .Where(x => x.Idvehiculo == id)
          .ToListAsync();

        viewModel.Servicios = servicios.Select(x => new ServicioUnidadViewModel
        {
          servicioUnidadId = x.Idvehiculoservicio,
          unidadId = x.Idvehiculo,
          detalle = x.Detalle!,
          fechaServicio = Convert.ToDateTime(x.Fechaservicio)

        }).ToList();


        viewModel.ServiciosJson = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Servicios);
      }

      return viewModel!;

    }

    /// <summary>
    /// Agrega o actualiza un vehículo en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel del vehículo.</param>
    /// <returns>Indica si la operación fue exitosa.</returns>
    /// <exception cref="Exception">Se lanza en caso de error.</exception>
    public async Task<bool> AgregarVehiculo(AgregarUnidadViewModel model, int empresaId)
    {
      Vehiculo? vehiculo = new();

      if (model.UnidadId > 0)
      {
        vehiculo = await context.Vehiculos.FirstOrDefaultAsync(x => x.IdVehiculo == model.UnidadId);

        if (vehiculo == null)
        {
          throw new Exception("No se encontro el vehículo");
        }
      }

      // validate if existe un vehiculo
      //validate if existe a vehicle with the same plates and is not the same vehicle that we are editing


      var existeVehiculo = await context.Vehiculos
        .FirstOrDefaultAsync(x => (x.Placas == model.Placas && x.EmpresaIdempresa == empresaId) && x.IdVehiculo != model.UnidadId);

      if (existeVehiculo != null)
      {
        throw new Exception("Ya existe un vehículo registrado con estas placas");
      }

      //validar el numero economico si ya existe
      var existeNumeroEconomico = await context.Vehiculos
        .FirstOrDefaultAsync(x => (x.Numeroeconomico == model.NumeroEconomico && x.EmpresaIdempresa == empresaId) && x.IdVehiculo != model.UnidadId);

      if (existeNumeroEconomico != null)
      {
        throw new Exception("Ya existe un vehículo registrado con este número económico");
      }

      vehiculo.Placas = model.Placas;
      vehiculo.Vin = model.Vin;
      vehiculo.Activo = (ulong?)(model.Activo ? 1 : 0);
      vehiculo.Fecharegistro = DateTime.Now;
      vehiculo.TipovehiculoIdtipovehiculo = (sbyte)model.TipoUnidadId;
      vehiculo.EmpresaIdempresa = empresaId;
      vehiculo.Numeropoliza = model.NumeroPoliza;
      vehiculo.Fechafinseguro = model.FechaFinSeguro;
      vehiculo.Marca = model.Marca;
      vehiculo.Modelo = model.Modelo;
      vehiculo.Numeroeconomico = model.NumeroEconomico;
      vehiculo.Numeromotor = model.NumeroMotor;


      if (model.UnidadId > 0)
      {
        context.Vehiculos.Entry(vehiculo);
      }
      else
      {
        context.Vehiculos.Add(vehiculo);
      }

      await context.SaveChangesAsync();
      if (!string.IsNullOrEmpty(model.ServiciosJson))
      {

        model.Servicios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServicioUnidadViewModel>>(model.ServiciosJson);
      }
      await AgregarServiciosVehiculo(model.Servicios!, vehiculo.IdVehiculo);

      return true;
    }

    /// <summary>
    /// actualiza la informacion de servicios de un vehiculo
    /// </summary>
    /// <param name="servicios"></param>
    /// <param name="idvehiculo"></param>
    /// <returns></returns>
    private async Task AgregarServiciosVehiculo(List<ServicioUnidadViewModel> servicios, int idvehiculo)
    {
      await context.Vehiculoservicios
        .Where(x => x.Idvehiculo == idvehiculo)
        .ExecuteDeleteAsync();

      if (servicios.Any())
      {
        foreach (var servicio in servicios)
        {
          Vehiculoservicio vehiculoservicio = new()
          {
            Idvehiculo = idvehiculo,
            Detalle = servicio.detalle,
            Fechaservicio = servicio.fechaServicio,
            Fecharegistro = DateTime.Now

          };

          context.Vehiculoservicios.Add(vehiculoservicio);
        }

        await context.SaveChangesAsync();

      }
    }
  }
}

