using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class FallaAuxilioVialController : Controller
    {
        private readonly ILogger<FallaAuxilioVialController> _logger;
        private readonly IFallaAuxilioVial fallaAuxilioVial;

        public FallaAuxilioVialController(ILogger<FallaAuxilioVialController> logger,
        IFallaAuxilioVial fallaAuxilioVial)

        {
            _logger = logger;
            this.fallaAuxilioVial = fallaAuxilioVial;

        }
        public async  Task<IActionResult> Index()
        {
            var falla_auxilio = await fallaAuxilioVial.GetFallaAuxilioVial();
            return View(falla_auxilio);
        }

        public async Task<IActionResult> AgregarFallaAuxilioVial(int id)
        {
            var agregarFallaModel = await fallaAuxilioVial.GetFallaAuxilioViewModel(id);
            return View(agregarFallaModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarFallaAuxilioVial(FallaAuxilioVialViewModel model)
        {
            try
            {
                var result = await fallaAuxilioVial.AgregarFallaAuxilioVial(model);

                if (result)
                {
                    return RedirectToAction("Index", "FallaAuxilioVial");
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
        public async Task<IActionResult> EliminarFalla([FromBody] FallaAuxilioVialViewModel model)
        {
            var falla_id = Convert.ToInt32(model.FallaId);
            try
            {
                await fallaAuxilioVial.EliminarFallaAuxilioVial(falla_id);

                return Ok(new { success = true });

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}
