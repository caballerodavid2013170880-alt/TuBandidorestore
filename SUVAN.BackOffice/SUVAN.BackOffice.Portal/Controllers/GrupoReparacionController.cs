using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.MantenimientoDetalleViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class GrupoReparacionController : Controller
    {
        private readonly ILogger<GrupoReparacionController> _logger;
        private readonly IGrupoReparacionService grupoService;

        public GrupoReparacionController(ILogger<GrupoReparacionController> logger,
        IGrupoReparacionService grupoService)

        {
            _logger = logger;
            this.grupoService = grupoService;

        }

        public async Task<IActionResult> Index()
        {
            var zona = await grupoService.GetGrupoReparacion();
            return View(zona);
        }

        public async Task<IActionResult> AgregarGrupoReparacion(int id)
        {
            var agregarModel = await grupoService.GetGrupoReparacionViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarGrupoReparacion(GrupoReparacionViewModel model)
        {
            try
            {
                var result = await grupoService.AgregarGrupoReparacion(model);

                if (result)
                {
                    return RedirectToAction("Index", "GrupoReparacion");
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
        public async Task<IActionResult> EliminarGrupoReparacion([FromBody] GrupoReparacionViewModel model)
        {
            try
            {
                await grupoService.EliminarGrupoReperacion(model.IdGrupo);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
