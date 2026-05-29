using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.VehiculoDetalleViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class BajaVehiculoController : Controller
    {
        private readonly ILogger<BajaVehiculoController> _logger;
        private readonly IBajaVehiculoService bajaService;

        public BajaVehiculoController(ILogger<BajaVehiculoController> logger,
            IBajaVehiculoService bajaService)

        {
            _logger = logger;
            this.bajaService = bajaService;

        }
        public async Task<IActionResult> Index()
        {
            var baja = await bajaService.GetBajaVehiculo();
            return View(baja);
        }

        public async Task<IActionResult> AgregarBajaVehiculo(int id)
        {
            var agregarModel = await bajaService.GetBajaVehiculoViewModel(id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarBajaVehiculo(BajaVehiViewModel model)
        {
            try
            {
                var result = await bajaService.AgregarBajaVehiculo(model);

                if (result)
                {
                    return RedirectToAction("Index", "BajaVehiculo");
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
        public async Task<IActionResult> EliminarBajaVehiculo([FromBody] BajaVehiViewModel model)
        {
            try
            {
                await bajaService.EliminarBajaVehiculo(model.IdBaja);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
