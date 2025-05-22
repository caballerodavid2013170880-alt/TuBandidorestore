using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MecanicoController : Controller
    {
        private readonly ILogger<MecanicoController> _logger;
        private readonly IMecanicoService mecanicoService;

        public MecanicoController(ILogger<MecanicoController> logger,
        IMecanicoService mecanicoService)

        {
            _logger = logger;
            this.mecanicoService = mecanicoService;

        }
        public async Task<IActionResult> Index()
        {
            var mecanico = await mecanicoService.GetMecanico();
            return View(mecanico);
        }

        public async Task<IActionResult> AgregarMecanico(int id)
        {
            var agregarModel = await mecanicoService.GetMecanicoViewModel(id);
            agregarModel.DepositoView = mecanicoService.ObtenerDeposito();
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMecanico(MecanicoViewModel model)
        {
            try
            {
                model.DepositoView = mecanicoService.ObtenerDeposito();

                var result = await mecanicoService.AgregarMecanico(model);

                if (result)
                {
                    return RedirectToAction("Index", "Mecanico");
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
        public async Task<IActionResult> EliminarMecanico([FromBody] MecanicoViewModel model)
        {
            try
            {
                await mecanicoService.EliminarMecanico(model.IdMecanico);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
