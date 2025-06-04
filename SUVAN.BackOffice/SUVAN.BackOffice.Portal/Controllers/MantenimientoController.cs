using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Portal.Helper;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MantenimientoController : Controller
    {
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
    }
}
