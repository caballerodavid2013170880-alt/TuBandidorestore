using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
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
            var zona = await zonaService.GetZona(User.GetEmpresaId());
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
                int IdEmpresa = User.GetEmpresaId();
                var result = await zonaService.AgregarZona(model, IdEmpresa);

                if (result)
                {
                    TempData["Mensaje"] = model.ZonaId == 0
                        ? "Registro insertado correctamente."
                        : "Registro actualizado correctamente.";

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
