using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class TipoServicioController : Controller
    {
        private readonly ILogger<TipoServicioController> _logger;
        private readonly ITipoServicioService tipoService;

        public TipoServicioController(ILogger<TipoServicioController> logger,
        ITipoServicioService servicioService)

        {
            _logger = logger;
            this.tipoService = servicioService;

        }
        public async Task <IActionResult> Index()
        {
            var tipoServicios = await tipoService.GetTipoServicio();
            return View(tipoServicios);
        }

        public async Task<IActionResult> AgregarTipoServicio(int id)
        {
            var agregarModel = await tipoService.GetTipoServicioViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTipoServicio(CausaMantenimientoViewModel.TipoServicioViewModel model)
        {
            try
            {
                var result = await tipoService.AgregarTipoServicio(model);

                if (result)
                {
                    return RedirectToAction("Index", "TipoServicio");
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
        public async Task<IActionResult> EliminarTipoServicio([FromBody] CausaMantenimientoViewModel.TipoServicioViewModel model)
        {
            try
            {
                await tipoService.EliminarTipoServicio(model.IdTiposervicio);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
