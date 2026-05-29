using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Configuracion
{
  public class TipoVehiculoService : ITipoVehiculoService
  {
    private readonly SuvanDbContext context;

    public TipoVehiculoService(SuvanDbContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// Obtiene los tipos de vehiculos desde la base de datos.
    /// </summary>
    /// <returns>Lista de tipos de vehiculos.</returns>
    public async Task<List<Tipovehiculo>> GetTipovehiculos()
    {
      var tipovehiculo = await context.Tipovehiculos.ToListAsync();
      return tipovehiculo!;
    }
    /// <summary>
    /// Obtiene los tipos de vehiculos desde la base de datos.
    /// </summary>
    /// <returns>Lista de tipos de vehiculos.</returns>
    public async Task<List<Tipovehiculo>> GetTipovehiculosActivo()
    {
      var tipovehiculo = await context.Tipovehiculos
        .Where(x => x.Activo == 1).ToListAsync();
      return tipovehiculo!;
    }

    /// <summary>
    /// Obtiene el ViewModel para agregar/editar un tipo de vehiculo.
    /// </summary>
    /// <param name="id">Identificador del tipo de vehiculo.</param>
    /// <returns>ViewModel para agregar/editar un tipo de vehiculo.</returns>
    public async Task<AgregarTipoUnidadViewModel> GetTipoVehiculoViewModel(int id)
    {
      var tipoVehiculo = await context.Tipovehiculos
        .FirstOrDefaultAsync(x => x.Idtipovehiculo == id);

      if (tipoVehiculo == null)
        return new AgregarTipoUnidadViewModel();

      return new AgregarTipoUnidadViewModel
      {
        TipoUnidadId = tipoVehiculo.Idtipovehiculo,
        Nombre = tipoVehiculo.Nombre!,
        Asientos = (int)tipoVehiculo.Asientos!,
        Activo = tipoVehiculo.Activo == 1
      };

    }

    /// <summary>
    /// Agrega o actualiza un tipo de vehiculo en la base de datos.
    /// </summary>
    /// <param name="model">ViewModel con los datos del tipo de vehiculo.</param>
    /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> AgregarTipoVehiculo(AgregarTipoUnidadViewModel model)
    {
      Tipovehiculo tipoVehiculo = new();

      if (model.TipoUnidadId > 0)
      {
        tipoVehiculo = await context.Tipovehiculos.FirstOrDefaultAsync(x => x.Idtipovehiculo == model.TipoUnidadId);

        if (tipoVehiculo == null)
        {
          throw new Exception("No se encontro el tipo de vehículo");
        }
      }

      // validate if existe one tipovehiculo with the same name adn is not the same empresa
      var existetipoVehiculo = await context.Tipovehiculos
        .FirstOrDefaultAsync(x => (x.Nombre == model.Nombre) && x.Idtipovehiculo != model.TipoUnidadId);

      if (existetipoVehiculo != null)
      {
        throw new Exception("Ya existe un tipo de vehículo con el mismo nombre");
      }

      tipoVehiculo.Nombre = model.Nombre;
      tipoVehiculo.Asientos = (int?)model.Asientos;
      tipoVehiculo.Activo = (ulong?)(model.Activo ? 1 : 0);
      tipoVehiculo.Fecharegistro = DateTime.Now;

      if (model.TipoUnidadId > 0)
      {
        context.Tipovehiculos.Entry(tipoVehiculo);
      }
      else
      {
        context.Tipovehiculos.Add(tipoVehiculo);
      }

      await context.SaveChangesAsync();

      return true;
    }
  }
}
