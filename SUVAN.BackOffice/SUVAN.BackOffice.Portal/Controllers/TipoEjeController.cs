using Microsoft.AspNetCore.Mvc;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using SUVAN.BackOffice.Service.Logistica;
using SUVAN.BackOffice.Models.ViewModel.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class TipoEjeController : Controller
    {
        private readonly ILogger<TipoEjeController> _logger;
        private readonly ITipoEjeService ejeService;

        public TipoEjeController(ILogger<TipoEjeController> logger,
            ITipoEjeService ejeService)

        {
            _logger = logger;
            this.ejeService = ejeService;

        }
        public async Task<IActionResult> Index()
        {
            var eje = await ejeService.GetTipoEje();
            return View(eje);
        }

        public async Task<IActionResult> AgregarTipoEje(int id)
        {
            var agregarModel = await ejeService.GetTipoEjeViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTipoEje(TipoEjeViewModel model)
        {
            try
            {
                var result = await ejeService.AgregarTipoEje(model);

                if (result)
                {
                    return RedirectToAction("Index", "TipoEje");
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
        public async Task<IActionResult> EliminarTipoEje([FromBody] TipoEjeViewModel model)
        {
            try
            {
                await ejeService.EliminarTipoEje(model.IdTipoEje);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
