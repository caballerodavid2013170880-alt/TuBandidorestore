using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MantenimientoController : Controller
    {
        private readonly ILogger<MantenimientoController> _logger;
        private readonly IMantenimientoService mantenimiento;

        public MantenimientoController(ILogger<MantenimientoController> logger,
        IMantenimientoService mantenimiento)

        {
            _logger = logger;
            this.mantenimiento = mantenimiento;

        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AgregarOrden()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OrdenDetalle()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OrdenMantenimiento() { 

            return PartialView("_ordenMantenimiento");
        }

        [HttpGet]
        public IActionResult DatosVehiculo()
        {

            return PartialView("_datosVehiculo");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTaller()
        {
            var talleres = await mantenimiento.ObtenerTaller(User.GetEmpresaId());
            return Ok(talleres);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMecanico(int tallerId)
        {
            var mecanicos = await mantenimiento.ObtenerMecanico(tallerId);
            return Json(mecanicos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTipoReparacion()
        {
            var reparacion = await mantenimiento.ObtenerTipoReparacion();
            return Json(reparacion);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCausaMantenimiento()
        {
            var causa = await mantenimiento.ObtenerCausaMantenimiento();
            return Json(causa);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTipoServicio()
        {
            var servicio = await mantenimiento.ObtenerTipoServicio();
            return Json(servicio);
        }

    }
}
