using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
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
            var agregarModel = await mecanicoService.GetMecanicoViewModel(id, User.GetEmpresaId());
            agregarModel.TallerView = mecanicoService.ObtenerTaller(agregarModel.IdDeposito);
            agregarModel.DepositoJson = JsonConvert.SerializeObject(agregarModel.DepositoView);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMecanico(MecanicoViewModel model)
        {
            try
            {
                model.TallerView = mecanicoService.ObtenerTaller(model.IdDeposito);

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
