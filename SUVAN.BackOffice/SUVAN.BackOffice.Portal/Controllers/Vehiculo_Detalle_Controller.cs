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

        //public async Task<IActionResult> AgregarVehiculoDetalle(int id)
        //{
        //    var agregarModel = await vehiculoService.GetVehiculoDetalleViewModel(id);
        //    return View(agregarModel);
        //}

        public async Task<IActionResult> NavegacionVehiculoDetalle(int id)
        {
            var agregarModel = await vehiculoService.GetVehiculoDetalleViewModel(id);
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
        public async Task<IActionResult> AgregarVehiculoDetalle(VehiculoDetalleViewModel model)
        {
            try
            {
                int IdEmpresa = User.GetEmpresaId();
                var result = await vehiculoService.AgregarVehiculoDetalle(model, IdEmpresa);

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
    }
}