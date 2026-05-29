using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;
using SUVAN.BackOffice.Service.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoEspecificacionesViewModel;

namespace SUVAN.BackOffice.Service.Configuracion
{
    public class VehiculoService : IVehiculoService
    {
        private readonly SuvanDbContext context;
        private readonly ITipoVehiculoService tipoVehiculoService;
        private readonly IMarcaService marcaService;
        private readonly IModeloService modeloService;

        public VehiculoService(SuvanDbContext context, ITipoVehiculoService tipoVehiculoService, IMarcaService marcaService, IModeloService modeloService)
        {
            this.context = context;
            this.tipoVehiculoService = tipoVehiculoService;
            this.marcaService = marcaService;
            this.modeloService = modeloService;
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
            List<Marca> marcas = await marcaService.GetMarca();
            List<Modelo> modelo = await modeloService.GetModelo();

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

            viewModel.Marcas = marcas.Select(x => new MarcaUnidadViewModel
            {
                IdMarca = x.IdMarca,
                DescripcionMarca = x.Descripcion!,
                Modelos = context.Modelos
                .Where(m => m.IdMarca == x.IdMarca)
                .Select(m => new ModeloUnidadViewModel
                {
                    IdModelo = m.IdModelo,
                    DescripcionModelo = m.Descripcion!
                }).ToList()
            }).ToList();

            viewModel.Modelos = modelo.Select(x => new ModeloUnidadViewModel
            {
                IdModelo = x.IdModelo,
                DescripcionModelo = x.Descripcion!
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
                viewModel.IdMarca = vehiculo?.IdMarca;
                viewModel.IdModelo = vehiculo?.IdModelo;

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

        public async Task<List<ModeloUnidadViewModel>> ObtenerModelo(int? marcaId)
        {
            var resultado = await (from t in context.Modelos
                                   where
                                   (t.IdMarca == marcaId)
                                   select new ModeloUnidadViewModel
                                   {
                                       IdModelo = t.IdModelo,
                                       DescripcionModelo = t.Descripcion

                                   }).ToListAsync();
            return resultado;
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
            vehiculo.IdMarca = model.IdMarca;
            vehiculo.IdModelo = model.IdModelo;


            if (model.UnidadId > 0)
            {
                context.Vehiculos.Entry(vehiculo);
            }
            else
            {
                context.Vehiculos.Add(vehiculo);
                await context.SaveChangesAsync();

                model.UnidadId = vehiculo.IdVehiculo;
            }

            await context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(model.ServiciosJson))
            {

                model.Servicios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServicioUnidadViewModel>>(model.ServiciosJson);
            }
            await AgregarServiciosVehiculo(model.Servicios!, vehiculo.IdVehiculo);
            await AgregarDetalle(model, vehiculo.IdVehiculo);

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

        public async Task<int> AgregarDetalle(AgregarUnidadViewModel model, int idvehiculo)
        {
            var detalleExistente = await context.VehiculoDetalles
                .FirstOrDefaultAsync(x => x.IdVehiculo == idvehiculo);

            if (detalleExistente != null)
            {
                detalleExistente.Carroceria = model.Vin;
                detalleExistente.NumeroMotor = model.NumeroMotor;
                detalleExistente.EconomicoAnterior = model.NumeroEconomico;

                await context.SaveChangesAsync();
                return detalleExistente.IdVehiculoDetalle;
            }
            else
            {
                VehiculoDetalle vehiculodetalle = new()
                {
                    IdVehiculo = idvehiculo,
                    Carroceria = model.Vin,
                    NumeroMotor = model.NumeroMotor,
                    EconomicoAnterior = model.NumeroEconomico
                };

                context.VehiculoDetalles.Add(vehiculodetalle);
                await context.SaveChangesAsync();

                return vehiculodetalle.IdVehiculoDetalle;
            }
        }


    }
}

