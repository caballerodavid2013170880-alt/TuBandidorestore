using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Ingresos;
using SUVAN.BackOffice.Service.Logistica;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class VehiculoEspecificacionesController : Controller
    {
        private readonly ILogger<VehiculoEspecificacionesController> _logger;
        private readonly IVehiculoEspecificacionesService especificacionesService;

        public VehiculoEspecificacionesController(ILogger<VehiculoEspecificacionesController> logger,
        IVehiculoEspecificacionesService especificacionesService)

        {
            _logger = logger;
            this.especificacionesService = especificacionesService;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new VehiculoEspecificacionesViewModel();
            model = await especificacionesService.ObtenerEspecificaciones(model); 
            model.MarcaJson = JsonConvert.SerializeObject(model.Marcas);
            return View(model);
        }

        public async Task<IActionResult> NavegacionVehiculoEspecifi(int id)
        {
            var agregarModel = await especificacionesService.GetVehiculoEspecifiViewModel(id);
            return View(agregarModel);
        }

        [HttpGet]
        public IActionResult DimensionCapacidad()
        {
            return PartialView("_dimensionCapacidad");
        }

        [HttpGet]
        public IActionResult MotorDesempeno()
        {

            return PartialView("_motorDesempeno");
        }

        [HttpGet]
        public IActionResult TransmisionTraccion()
        {

            return PartialView("_transmisionTraccion");
        }

        [HttpGet]
        public IActionResult DatosAdicionales()
        {

            return PartialView("_datosAdicionales");
        }

        [HttpPost]
        public async Task<IActionResult> Index(VehiculoEspecificacionesViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await especificacionesService.ObtenerEspecificaciones(model);
                return View(result);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> NavegacionVehiculoEspecificaciones(VehiculoEspecificacionesViewModel model)
        {
            try
            {

                var result = await especificacionesService.AgregarVehiculoEspecificaciones(model);

                if (result)
                {
                    return RedirectToAction("Index", "VehiculoEspecificaciones");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
