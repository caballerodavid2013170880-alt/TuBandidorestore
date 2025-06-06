using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;

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
        public async Task<IActionResult> ObtenerMecanico()
        {
            var mecanicos = await mantenimiento.ObtenerMecanico(User.GetEmpresaId());
            return Ok(mecanicos);
        }
    }
}
