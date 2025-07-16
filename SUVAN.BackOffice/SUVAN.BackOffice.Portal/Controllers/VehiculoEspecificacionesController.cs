using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _env;

        public VehiculoEspecificacionesController(ILogger<VehiculoEspecificacionesController> logger,
        IVehiculoEspecificacionesService especificacionesService, IWebHostEnvironment env)

        {
            _logger = logger;
            this.especificacionesService = especificacionesService;
             _env = env;
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

        [HttpGet]
        public IActionResult Imagen()
        {

            return PartialView("_imagen");
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
        public async Task<IActionResult> NavegacionVehiculoEspecificaciones(VehiculoEspecificacionesViewModel model, List<IFormFile> archivosImagen)
        {
            try
            {
                if (archivosImagen != null && archivosImagen.Any())
                {
                    await especificacionesService.GuardarImagenesAsync(model, archivosImagen, _env.WebRootPath);
                }

                var result = await especificacionesService.AgregarVehiculoEspecificaciones(model);

                if (result)
                {
                    return RedirectToAction("Index", "VehiculoEspecificaciones");
                }

                var resultModel = await especificacionesService.ObtenerEspecificaciones(model);
                return View("Index", resultModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var resultModel = await especificacionesService.ObtenerEspecificaciones(model);
                return View("Index", resultModel);
            }
        }

    }
}
