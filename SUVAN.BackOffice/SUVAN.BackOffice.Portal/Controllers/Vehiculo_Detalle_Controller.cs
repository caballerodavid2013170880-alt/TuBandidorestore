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
        public async Task<IActionResult> ObtenerDetalleVehiculo(int idVehiculoDetalle)
        {
            var detalle = await vehiculoService.ObtenerDetalleModal(idVehiculoDetalle);
            return Json(detalle);
        }
    }
}