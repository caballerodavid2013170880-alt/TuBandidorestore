using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class TallerController : Controller
    {
        private readonly ILogger<TallerController> _logger;
        private readonly ITallerService taller;

        public TallerController(ILogger<TallerController> logger,
        ITallerService talleres)

        {
            _logger = logger;
            this.taller = talleres;

        }

        public async Task<IActionResult> Index()
        {
            var t = await taller.GetTaller();
            return View(t);
        }

        public async Task<IActionResult> AgregarTaller(int id)
        {
            var agregarModel = await taller.GetTallerViewModel(id);
            agregarModel.ZonaView = taller.ObtenerZona();
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTaller(TallerViewModel model)
        {
            try
            {
                model.ZonaView = taller.ObtenerZona();

                var result = await taller.AgregarTaller(model);

                if (result)
                {
                    return RedirectToAction("Index", "Taller");
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
        public async Task<IActionResult> EliminarTaller([FromBody] TallerViewModel model)
        {
            try
            {
                await taller.EliminarTaller(model.IdTaller);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
