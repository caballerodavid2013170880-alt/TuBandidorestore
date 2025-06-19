using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Service.Logistica
{
    public class VehiculoDetalleService : IVehiculoDetalleService
    {
        private readonly SuvanDbContext context;

        public VehiculoDetalleService(SuvanDbContext context)
        {
            this.context = context;
        }

        public async Task<List<VehiculoDetalle>> GetVehiculoDetalle()
        {

            var vehiculo = await context.VehiculoDetalles.ToListAsync();

            return vehiculo!;
        }

        /// <summary>
        /// Obtiene el ViewModel del VehiculoDetalle específico.
        /// </summary>
        /// <param name="id">Identificador del VehiculoDetalle.</param>
        /// <returns>ViewModel para el VehiculoDetalle especifico.</returns>
        public async Task<VehiculoDetalleViewModel> GetVehiculoDetalleViewModel(int id)
        {
            VehiculoDetalleViewModel vRet = new VehiculoDetalleViewModel();
            var vehiculo = await context.VehiculoDetalles.FirstOrDefaultAsync(x => x.IdVehiculoDetalle == id);

            if (vehiculo == null)
                return vRet;
            else
            {
                vRet = new VehiculoDetalleViewModel
                {
                    IdVehiculoDetalle = vehiculo.IdVehiculoDetalle,
                    IdVehiculo = vehiculo.IdVehiculo,
                    IdTipoVehiculo = vehiculo.IdTipoVehiculo,
                    IdMarca = vehiculo.IdMarca,
                    IdZona = vehiculo.IdZona,
                    IdDeposito = vehiculo.IdDeposito,
                    IdEspecificacion = vehiculo.IdEspecificacion,
                    IdModelo = vehiculo.IdModelo,
                    IdCognos = vehiculo.IdCognos,
                    IdTipoEje = vehiculo.IdTipoEje,
                    IdNegocio = vehiculo.IdNegocio,
                    Area = vehiculo.Area,
                    AnioVehiculo = vehiculo.AnioVehiculo,
                    ColorVehiculo = vehiculo.ColorVehiculo,
                    TieneRotulo = vehiculo.TieneRotulo,
                    PlacaPe = vehiculo.PlacaPe,
                    NumeroSerie = vehiculo.NumeroSerie,
                    NumeroMotor = vehiculo.NumeroMotor,
                    Carroceria = vehiculo.Carroceria,
                    TarjetaCirculacion = vehiculo.TarjetaCirculacion,
                    Gasolina = vehiculo.Gasolina,
                    Encierro = vehiculo.Encierro,
                    CopiaFactura = vehiculo.CopiaFactura,
                    CopiaTarjetaCir = vehiculo.CopiaTarjetaCir,
                    CopiaPlaca = vehiculo.CopiaPlaca,
                    CopiaVerificacion = vehiculo.CopiaVerificacion,
                    CopiaPolizaSeguro = vehiculo.CopiaPolizaSeguro,
                    NoCircula = vehiculo.NoCircula,
                    DnoCircula = vehiculo.DnoCircula,
                    Proveedor = vehiculo.Proveedor,
                    FechaCompra = vehiculo.FechaCompra,
                    NumeroFactura = vehiculo.NumeroFactura,
                    CostoVehiculo = vehiculo.CostoVehiculo,
                    TarifaVehicular = vehiculo.TarifaVehicular,
                    NombreTarifaVehicular = vehiculo.NombreTarifaVehicular,
                    KilometrajeAcumulado = vehiculo.KilometrajeAcumulado,
                    StVehiculo = vehiculo.StVehiculo,
                    FechaBaja = vehiculo.FechaBaja,
                    ColorInterior = vehiculo.ColorInterior,
                    RegFed = vehiculo.RegFed,
                    EdregPl = vehiculo.EdregPl,
                    ColEst = vehiculo.ColEst,
                    TieneCaja = vehiculo.TieneCaja,
                    NecesitaRemolque = vehiculo.NecesitaRemolque,
                    VehiculoRelevo = vehiculo.VehiculoRelevo,
                    Rentado = vehiculo.Rentado,
                    ColRuta = vehiculo.ColRuta,
                    IdCausaBaja = vehiculo.IdCausaBaja,
                    ValorRecuperacion = vehiculo.ValorRecuperacion,
                    KilometrajeGarantia = vehiculo.KilometrajeGarantia,
                    MesesGarantia = vehiculo.MesesGarantia,
                    EconomicoAnterior = vehiculo.EconomicoAnterior,
                    LocFor = vehiculo.LocFor,
                    IdBaja = vehiculo.IdBaja,
                    PesoMinimo = vehiculo.PesoMinimo,
                    PesoMaximo = vehiculo.PesoMaximo,
                    VolumenMinimo = vehiculo.VolumenMinimo,
                    VolumenMaximo = vehiculo.VolumenMaximo,
                    TipoLicenciaRequerida = vehiculo.TipoLicenciaRequerida,
                    PermisoCargaAceite = vehiculo.PermisoCargaAceite,
                    VigenciaPermisoAceite = vehiculo.VigenciaPermisoAceite,
                    VigenciaTarjetaCirculacion = vehiculo.VigenciaTarjetaCirculacion,
                    Asignado = vehiculo.Asignado,
                    TotalVehiculo = vehiculo.TotalVehiculo,
                    UsuarioCaptura = vehiculo.UsuarioCaptura
                };
            }

            return vRet;
        }

        /// <summary>
        /// Agrega o actualiza un registro Vehiculo Detalle en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de Vehiculo Detalle.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarVehiculoDetalle(VehiculoDetalleViewModel model, int IdEmpresa)
        {
            VehiculoDetalle vehiculo;

            if (model.IdVehiculoDetalle > 0)
            {
                vehiculo = await context.VehiculoDetalles.FirstOrDefaultAsync(x => x.IdVehiculoDetalle == model.IdVehiculoDetalle);

                if (vehiculo == null)
                    throw new Exception("No se encontro el Vehiculo Detalle");

            }
            else
            {
                vehiculo = new VehiculoDetalle();
            }

            // Valida si el Detalle Vehiculo es esta en la misma empresa
            var vehiculoExistente = await context.VehiculoDetalles.FirstOrDefaultAsync(x =>
                x.IdVehiculoDetalle! == model.IdVehiculoDetalle! &&
                x.IdDepositoNavigation.IdEmpresa == IdEmpresa);

            if (vehiculoExistente is not null)
                throw new Exception("Ya existe un Vehiculo con el mismo nombre");

            vehiculo.IdVehiculo = model.IdVehiculo;
            vehiculo.IdTipoVehiculo = model.IdTipoVehiculo;
            vehiculo.IdMarca = model.IdMarca;
            vehiculo.IdZona = model.IdZona;
            vehiculo.IdDeposito = model.IdDeposito;
            vehiculo.IdEspecificacion = model.IdEspecificacion;
            vehiculo.IdModelo = model.IdModelo;
            vehiculo.IdCognos = model.IdCognos;
            vehiculo.IdTipoEje = model.IdTipoEje;
            vehiculo.IdNegocio = model.IdNegocio;
            vehiculo.Area = model.Area;
            vehiculo.AnioVehiculo = model.AnioVehiculo;
            vehiculo.ColorVehiculo = model.ColorVehiculo;
            vehiculo.TieneRotulo = model.TieneRotulo;
            vehiculo.PlacaPe = model.PlacaPe;
            vehiculo.NumeroSerie = model.NumeroSerie;
            vehiculo.NumeroMotor = model.NumeroMotor;
            vehiculo.Carroceria = model.Carroceria;
            vehiculo.TarjetaCirculacion = model.TarjetaCirculacion;
            vehiculo.Gasolina = model.Gasolina;
            vehiculo.Encierro = model.Encierro;
            vehiculo.CopiaFactura = model.CopiaFactura;
            vehiculo.CopiaTarjetaCir = model.CopiaTarjetaCir;
            vehiculo.CopiaPlaca = model.CopiaPlaca;
            vehiculo.CopiaVerificacion = model.CopiaVerificacion;
            vehiculo.CopiaPolizaSeguro = model.CopiaPolizaSeguro;
            vehiculo.NoCircula = model.NoCircula;
            vehiculo.DnoCircula = model.DnoCircula;
            vehiculo.Proveedor = model.Proveedor;
            vehiculo.FechaCompra = model.FechaCompra;
            vehiculo.NumeroFactura = model.NumeroFactura;
            vehiculo.CostoVehiculo = model.CostoVehiculo;
            vehiculo.TarifaVehicular = model.TarifaVehicular;
            vehiculo.NombreTarifaVehicular = model.NombreTarifaVehicular;
            vehiculo.KilometrajeAcumulado = model.KilometrajeAcumulado;
            vehiculo.StVehiculo = model.StVehiculo;
            vehiculo.FechaBaja = model.FechaBaja;
            vehiculo.ColorInterior = model.ColorInterior;
            vehiculo.RegFed = model.RegFed;
            vehiculo.EdregPl = model.EdregPl;
            vehiculo.ColEst = model.ColEst;
            vehiculo.TieneCaja = model.TieneCaja;
            vehiculo.NecesitaRemolque = model.NecesitaRemolque;
            vehiculo.VehiculoRelevo = model.VehiculoRelevo;
            vehiculo.Rentado = model.Rentado;
            vehiculo.ColRuta = model.ColRuta;
            vehiculo.IdCausaBaja = model.IdCausaBaja;
            vehiculo.ValorRecuperacion = model.ValorRecuperacion;
            vehiculo.KilometrajeGarantia = model.KilometrajeGarantia;
            vehiculo.MesesGarantia = model.MesesGarantia;
            vehiculo.EconomicoAnterior = model.EconomicoAnterior;
            vehiculo.LocFor = model.LocFor;
            vehiculo.IdBaja = model.IdBaja;
            vehiculo.PesoMinimo = model.PesoMinimo;
            vehiculo.PesoMaximo = model.PesoMaximo;
            vehiculo.VolumenMinimo = model.VolumenMinimo;
            vehiculo.VolumenMaximo = model.VolumenMaximo;
            vehiculo.TipoLicenciaRequerida = model.TipoLicenciaRequerida;
            vehiculo.PermisoCargaAceite = model.PermisoCargaAceite;
            vehiculo.VigenciaPermisoAceite = model.VigenciaPermisoAceite;
            vehiculo.VigenciaTarjetaCirculacion = model.VigenciaTarjetaCirculacion;
            vehiculo.Asignado = model.Asignado;
            vehiculo.TotalVehiculo = model.TotalVehiculo;
            vehiculo.UsuarioCaptura = model.UsuarioCaptura;

            if (model.IdVehiculoDetalle > 0)
            {
                context.VehiculoDetalles.Entry(vehiculo);

                await context.SaveChangesAsync();
            }
            else
            {
                context.VehiculoDetalles.Add(vehiculo);
                await context.SaveChangesAsync();

            }
            return true;
        }

        /// <summary>
        /// Elimina un registro de Vehiculo Detalle en la base de datos.
        /// </summary>
        /// <param name="IdVehiculo">Identificador del Vehiculo Detalle.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarVehiculoDetalle(int IdVehiculo)
        {
            var vehiculo = await context.VehiculoDetalles.FirstOrDefaultAsync(x => x.IdVehiculoDetalle == IdVehiculo);

            if (vehiculo is null)
            {
                throw new Exception("No se encontro el Vehiculo Detalle");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.VehiculoDetalles
              .Where(x => x.IdVehiculo == IdVehiculo)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

        public async Task<List<TipoVehiculoViewModel>> ObtenerTipoVehiculo()
        {
            var resultado = await (from t in context.Tipovehiculos
                                   select new TipoVehiculoViewModel
                                   {
                                       TipoUnidadId = t.Idtipovehiculo,
                                       Nombre = t.Nombre,

                                   }).ToListAsync();
            return resultado;
        }

        public async Task<List<MarcaViewModel>> ObtenerMarcas()
        {
            var resultado = await (from t in context.Marcas
                                   select new MarcaViewModel
                                   {
                                       IdMarca = t.IdMarca,
                                       Descripcion = t.Descripcion,

                                   }).ToListAsync();
            return resultado;
        }
    }
}