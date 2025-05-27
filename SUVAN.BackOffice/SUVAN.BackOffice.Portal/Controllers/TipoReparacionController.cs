using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class TipoReparacionController : Controller
    {
        private readonly ILogger<TipoReparacionController> _logger;
        private readonly ITipoReparacionService reparacionService;

        public TipoReparacionController(ILogger<TipoReparacionController> logger,
        ITipoReparacionService tipoService)

        {
            _logger = logger;
            this.reparacionService = tipoService;

        }
        public async Task<IActionResult> Index()
        {
            var reparacion = await reparacionService.GetTipoReparacion();
            return View(reparacion);
        }

        public async Task<IActionResult> AgregarTipoReparacion(int id)
        {
            var agregarModel = await reparacionService.GetTipoReparacionViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTipoReparacion(MantenimientoDetalleViewModel.TipoReparacionViewModel model)
        {
            try
            {
                var result = await reparacionService.AgregarTipoReparacion(model);

                if (result)
                {
                    return RedirectToAction("Index", "TipoReparacion");
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
        public async Task<IActionResult> EliminarTipoReparacion([FromBody] MantenimientoDetalleViewModel.TipoReparacionViewModel model)
        {
            try
            {
                await reparacionService.EliminarTipoReparacion(model.IdTipoReparacion);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}
