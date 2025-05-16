using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class ZonaController : Controller
    {
        private readonly ILogger<ZonaController> _logger;
        private readonly IZonaService zonaService;

        public ZonaController(ILogger<ZonaController> logger,
        IZonaService zonaService)

        {
            _logger = logger;
            this.zonaService = zonaService;

        }
        public async Task<IActionResult> Index()
        {
            var zona = await zonaService.GetZona();
            return View(zona);
        }

        public async Task<IActionResult> AgregarZona(int id)
        {
            var agregarModel = await zonaService.GetZonaViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarZona(ZonaViewModel model)
        {
            try
            {
                var result = await zonaService.AgregarZona(model);

                if (result)
                {
                    return RedirectToAction("Index", "Zona");
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
        public async Task<IActionResult> EliminarZona([FromBody] ZonaViewModel model)
        {
            try
            {
                await zonaService.EliminarZona(model.ZonaId);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
