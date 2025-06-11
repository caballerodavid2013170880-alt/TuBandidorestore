using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class CausaMantenimientoController : Controller
    {
        private readonly ILogger<CausaMantenimientoController> _logger;
        private readonly ICausaMantenimientoService causaService;

        public CausaMantenimientoController(ILogger<CausaMantenimientoController> logger,
        ICausaMantenimientoService causaService)

        {
            _logger = logger;
            this.causaService = causaService;

        }
        public async Task<IActionResult> Index()
        {
            var causa = await causaService.GetCausaMantenimiento();
            return View(causa);
        }

        public async Task<IActionResult> AgregarCausaMantenimiento(int id)
        {
            var agregarModel = await causaService.GetCausaMantenimientoViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCausaMantenimiento(CausaMantenimientoViewModel model)
        {
            try
            {
                var result = await causaService.AgregarCausaMantenimiento(model);

                if (result)
                {
                    return RedirectToAction("Index", "CausaMantenimiento");
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
        public async Task<IActionResult> EliminarCausaMantenimiento([FromBody] CausaMantenimientoViewModel model)
        {
            try
            {
                await causaService.EliminarCausaMantenimiento(model.IdCausamantenimiento);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
