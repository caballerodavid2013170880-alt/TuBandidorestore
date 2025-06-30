using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SUVAN.BackOffice.Portal.Models;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class VehiculoDetalleController : Controller
    {
        private readonly ILogger<VehiculoDetalleController> _logger;
        private readonly IVehiculoDetalleService vehiculoService;

        public VehiculoDetalleController(ILogger<VehiculoDetalleController> logger,
        IVehiculoDetalleService vehiculoService)

        {
            _logger = logger;
            this.vehiculoService = vehiculoService;

        }
        public async Task<IActionResult> Index()
        {
            var vehiculo = await vehiculoService.GetVehiculoDetalle();
            return View(vehiculo);
        }
        public async Task<IActionResult> NavegacionVehiculoDetalle(int id)
        {
            var agregarModel = await vehiculoService.GetVehiculoDetalleViewModel(id);
            agregarModel.MarcasJson = JsonConvert.SerializeObject(agregarModel.Marcas);
            agregarModel.ZonaJson = JsonConvert.SerializeObject(agregarModel.Zonas);
            return View(agregarModel);
        }

        public IActionResult DatosGenerales()
        {
            return PartialView("_datosGenerales");
        }
        public IActionResult UbicacionDocumentacion()
        {

            return PartialView("_ubicacionDocumentacion");
        }
        public IActionResult CompraCosto()
        {

            return PartialView("_compraCosto");
        }
        public IActionResult GarantiaEstado()
        {

            return PartialView("_garantiaEstado");
        }

        public IActionResult EspecificacionesTecnicas()
        {

            return PartialView("_especificacionesTecnicas");
        }

        public IActionResult PermisosLicencias()
        {

            return PartialView("_permisosLicencias");
        }

        [HttpPost]
        public async Task<IActionResult> NavegacionVehiculoDetalle(VehiculoDetalleViewModel model, int IdEmpresa)
        {
            try
            {
                IdEmpresa = User.GetEmpresaId();
                var Usuario = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;

                if (!ModelState.IsValid)
                {
                    await vehiculoService.CompletarCamposError(model);

                    return View(model);
                }

                var result = await vehiculoService.AgregarVehiculoDetalle(model, IdEmpresa, Usuario);

                if (result)
                {
                    return RedirectToAction("Index", "VehiculoDetalle");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarVehiculoDetalle([FromBody] VehiculoDetalleViewModel model)
        {
            try
            {
                await vehiculoService.EliminarVehiculoDetalle(model.IdVehiculoDetalle);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTipoVehiculo()
        {
            var tipo = await vehiculoService.ObtenerTipoVehiculo();
            return Json(tipo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMarca()
        {
            var tipo = await vehiculoService.ObtenerMarcas();
            return Json(tipo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTipoEje()
        {
            var eje = await vehiculoService.ObtenerTipoEje();
            return Json(eje);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerBajaVehi()
        {
            var baja = await vehiculoService.ObtenerBajaVehi();
            return Json(baja);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCausaBaja()
        {
            var causa = await vehiculoService.ObtenerCausaBaja();
            return Json(causa);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerModelo(int idMarca)
        {
            var modelo =  await vehiculoService.ObtenerModelo(idMarca);
            return Json(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerZona()
        {
            var zona = await vehiculoService.ObtenerZona(User.GetEmpresaId());
            return Json(zona);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDeposito(int idZona)
        {
            var deposito = await vehiculoService.ObtenerDepositos(idZona);
            return Json(deposito);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerVehiculo()
        {
            var vehiculo = await vehiculoService.ObtenerVehiculo(User.GetEmpresaId());
            return Json(vehiculo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleVehiculo(int idVehiculoDetalle)
        {
            var detalle = await vehiculoService.ObtenerDetalleModal(idVehiculoDetalle);
            return Json(detalle);
        }
    }
}