using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Ingresos;
using SUVAN.BackOffice.Service.Logistica;

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
        public async Task<IActionResult> Index()
        {
            var model = await especificacionesService.GetVehiculoEspecifi();
            model.MarcaJson = JsonConvert.SerializeObject(model.Marcas);
            return View(model);
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

                var result = await especificacionesService.ObtenerEspecificaciones(model, model.IdModelo);
                return View(result);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
