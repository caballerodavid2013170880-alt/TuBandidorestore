using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

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

            var vehiculo = await context.VehiculoDetalles.Include(x => x.IdVehiculoNavigation).Include(x => x.IdMarcaNavigation)
                .Include(x => x.IdZonaNavigation).Include(x => x.IdDepositoNavigation).ToListAsync();

            return vehiculo!;
        }

        /// <summary>
        /// Obtiene el ViewModel del VehiculoDetalle específico.
        /// </summary>
        /// <param name="id">Identificador del VehiculoDetalle.</param>
        /// <returns>ViewModel para el VehiculoDetalle especifico.</returns>
        public async Task<VehiculoDetalleViewModel> GetVehiculoDetalleViewModel(int id)
        {
            var vehiculo = await context.VehiculoDetalles.Include(v => v.IdMarcaNavigation).Include(v => v.IdModeloNavigation).
                Include(v => v.IdTipoVehiculoNavigation).Include(v => v.IdBajaNavigation).Include(v => v.IdTipoEjeNavigation).
                Include(v => v.IdCausaBajaNavigation).Include(v => v.IdDepositoNavigation).Include(v => v.IdZonaNavigation).
                Include(v => v.IdVehiculoNavigation).FirstOrDefaultAsync(x => x.IdVehiculoDetalle == id);

            if (vehiculo == null)
                return new VehiculoDetalleViewModel();

            return new VehiculoDetalleViewModel
            {
                IdMarca = vehiculo.IdMarca,
                DescripcionMarca = vehiculo.IdMarcaNavigation.Descripcion,
                IdTipoVehiculo = vehiculo.IdTipoVehiculo,
                NombreTipoV = vehiculo.IdTipoVehiculoNavigation.Nombre,
                IdModelo = vehiculo.IdModelo,
                DescripcionModelo = vehiculo.IdModeloNavigation.Descripcion,
                IdBaja = vehiculo.IdBaja,
                DescripcionBaja = vehiculo.IdBajaNavigation.Descripcion,
                IdCausaBaja = vehiculo.IdCausaBaja,
                DescripcionCausaBaja = vehiculo.IdCausaBajaNavigation.Descripcion,
                IdTipoEje = vehiculo.IdTipoEje,
                DescripcionEje = vehiculo.IdTipoEjeNavigation.Descripcion,
                IdZona = vehiculo.IdZona,
                NombreZona = vehiculo.IdZonaNavigation.NombreZona,
                IdDeposito = vehiculo.IdDeposito,
                NombreDeposito = vehiculo.IdDepositoNavigation.DepositoNombre,
                IdVehiculo = vehiculo.IdVehiculo,
                PlacasVehiculo = vehiculo.IdVehiculoNavigation.Placas,
                IdVehiculoDetalle = vehiculo.IdVehiculoDetalle,
                IdEspecificacion = vehiculo.IdEspecificacion,
                IdCognos = vehiculo.IdCognos,
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
                ValorRecuperacion = vehiculo.ValorRecuperacion,
                KilometrajeGarantia = vehiculo.KilometrajeGarantia,
                MesesGarantia = vehiculo.MesesGarantia,
                EconomicoAnterior = vehiculo.EconomicoAnterior,
                LocFor = vehiculo.LocFor,
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

        /// <summary>
        /// Agrega o actualiza un registro Vehiculo Detalle en la base de datos.
        /// </summary>
        /// <param name="model">ViewModel con los datos de Vehiculo Detalle.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AgregarVehiculoDetalle(VehiculoDetalleViewModel model, int IdEmpresa, string Usuario)
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

            // Valida si el Detalle Vehiculo teien el mismo Carroceria/VIN en la misma empresa
            var vehiculoExistente = await context.VehiculoDetalles.FirstOrDefaultAsync(x =>
                x.Carroceria!.Trim().ToLower() == model.Carroceria!.Trim().ToLower() &&
                x.IdDepositoNavigation.IdEmpresa == IdEmpresa &&
                x.IdVehiculoDetalle != model.IdVehiculoDetalle);

            if (vehiculoExistente is not null)
                throw new Exception("Ya existe un Vehiculo con el mismo VIN");

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
            vehiculo.UsuarioCaptura = Usuario;

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
        /// <param name="IdVehiculoDetalle">Identificador del Vehiculo Detalle.</param>
        /// <returns>True si la operación fue exitosa, de lo contrario, lanza una excepción.</returns>
        /// <exception cref="Exception"></exception>

        public async Task<bool> EliminarVehiculoDetalle(int IdVehiculoDetalle)
        {
            var vehiculo = await context.VehiculoDetalles.FirstOrDefaultAsync(x => x.IdVehiculoDetalle == IdVehiculoDetalle);

            if (vehiculo is null)
            {
                throw new Exception("No se encontro el Vehiculo Detalle");
            }

            // Desactivar temporamente el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            var delete = await context.VehiculoDetalles
              .Where(x => x.IdVehiculoDetalle == IdVehiculoDetalle)
              .ExecuteDeleteAsync();

            await context.SaveChangesAsync();

            // Volver a activar el seguimiento de entidades relacionadas
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            return true;
        }

        public async Task CompletarCamposError(VehiculoDetalleViewModel model)
        {
            if (model == null || model.IdVehiculoDetalle == 0)
                return;

            var datos = await GetVehiculoDetalleViewModel(model.IdVehiculoDetalle);

            model.NombreTipoV = datos.NombreTipoV;
            model.DescripcionMarca = datos.DescripcionMarca;
            model.DescripcionModelo = datos.DescripcionModelo;
            model.DescripcionBaja = datos.DescripcionBaja;
            model.DescripcionCausaBaja = datos.DescripcionCausaBaja;
            model.DescripcionEje = datos.DescripcionEje;
            model.NombreZona = datos.NombreZona;
            model.NombreDeposito = datos.NombreDeposito;
            model.PlacasVehiculo = datos.PlacasVehiculo;
        }

        public async Task<List<VehiculoDetalleViewModel>> ObtenerDetalleModal(int idVehiculoDetalle)
        {
            var resultado = await context.VehiculoDetalles.Where(v => v.IdVehiculoDetalle == idVehiculoDetalle).Include(v => v.IdVehiculoNavigation).Include(v => v.IdMarcaNavigation)
                .Include(v => v.IdModeloNavigation).Include(v => v.IdTipoVehiculoNavigation).Include(v => v.IdZonaNavigation)
                .Include(v => v.IdDepositoNavigation).Include(v => v.IdBajaNavigation).Include(v => v.IdCausaBajaNavigation)
                .Select(vehiculo => new VehiculoDetalleViewModel
                {
                    IdVehiculoDetalle = vehiculo.IdVehiculoDetalle,
                    IdVehiculo = vehiculo.IdVehiculo,
                    PlacasVehiculo = vehiculo.IdVehiculoNavigation.Placas,
                    IdMarca = vehiculo.IdMarca,
                    DescripcionMarca = vehiculo.IdMarcaNavigation.Descripcion,
                    IdModelo = vehiculo.IdModelo,
                    DescripcionModelo = vehiculo.IdModeloNavigation.Descripcion,
                    IdTipoVehiculo = vehiculo.IdTipoVehiculo,
                    NombreTipoV = vehiculo.IdTipoVehiculoNavigation.Nombre,
                    Carroceria = vehiculo.Carroceria,
                    IdZona = vehiculo.IdZona,
                    NombreZona = vehiculo.IdZonaNavigation.NombreZona,
                    IdDeposito = vehiculo.IdDeposito,
                    NombreDeposito = vehiculo.IdDepositoNavigation.DepositoNombre,
                    CopiaFactura = vehiculo.CopiaFactura,
                    CopiaVerificacion = vehiculo.CopiaVerificacion,
                    Proveedor = vehiculo.Proveedor,
                    FechaCompra = vehiculo.FechaCompra,
                    CostoVehiculo = vehiculo.CostoVehiculo,
                    IdBaja = vehiculo.IdBaja,
                    DescripcionBaja = vehiculo.IdBajaNavigation.Descripcion,
                    IdCausaBaja = vehiculo.IdCausaBaja,
                    DescripcionCausaBaja = vehiculo.IdCausaBajaNavigation.Descripcion,
                    MesesGarantia = vehiculo.MesesGarantia,
                    FechaBaja = vehiculo.FechaBaja,
                    PesoMinimo = vehiculo.PesoMinimo,
                    PesoMaximo = vehiculo.PesoMaximo,
                    VolumenMinimo = vehiculo.VolumenMinimo,
                    VolumenMaximo = vehiculo.VolumenMaximo,
                    TipoLicenciaRequerida = vehiculo.TipoLicenciaRequerida,
                    TarjetaCirculacion = vehiculo.TarjetaCirculacion,
                    VigenciaTarjetaCirculacion = vehiculo.VigenciaTarjetaCirculacion

                })
                .ToListAsync();

            return resultado;
        }


    }
}