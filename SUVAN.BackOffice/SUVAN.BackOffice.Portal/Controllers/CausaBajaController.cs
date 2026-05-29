using Microsoft.AspNetCore.Mvc;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;
using SUVAN.BackOffice.Service.Logistica;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class CausaBajaController : Controller
    {
        private readonly ILogger<CausaBajaController> _logger;
        private readonly ICausaBajaService causaService;

        public CausaBajaController(ILogger<CausaBajaController> logger,
            ICausaBajaService causaService)

        {
            _logger = logger;
            this.causaService = causaService;

        }
        public async Task<IActionResult> Index()
        {
            var causa = await causaService.GetCausaBaja();
            return View(causa);
        }

        public async Task<IActionResult> AgregarCausaBajaVehiculo(int id)
        {
            var agregarModel = await causaService.GetCausaBajaViewModel(id);
            agregarModel.BajaVehiculoView = causaService.ObtenerBajaVehiculo();
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCausaBajaVehiculo(CausaBajaViewModel model)
        {
            try
            {
                model.BajaVehiculoView = causaService.ObtenerBajaVehiculo();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await causaService.AgregarCausaBajaVehiculo(model);

                if (result)
                {
                    return RedirectToAction("Index", "CausaBaja");
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
        public async Task<IActionResult> EliminarCausaBajaVehiculo([FromBody] CausaBajaViewModel model)
        {
            try
            {
                await causaService.EliminarCausaBajaVehiculo(model.IdCausaBaja);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
