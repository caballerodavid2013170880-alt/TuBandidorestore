using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;

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
        // 1407 evitar conflictos con depostios disponibles public async Task<IActionResult> Index()
        // 1407 evitar conflictos con depostios disponibles {
        // 1407 evitar conflictos con depostios disponibles var mecanico = await mecanicoService.GetMecanico(User.GetEmpresaId());
        // 1407 evitar conflictos con depostios disponibles return View(mecanico);
        // 1407 evitar conflictos con depostios disponibles }

        // 1407 evitar conflictos con depostios disponibles public async Task<IActionResult> AgregarMecanico(int id)
        // 1407 evitar conflictos con depostios disponibles {
        // 1407 evitar conflictos con depostios disponibles var agregarModel = await mecanicoService.GetMecanicoViewModel(id, User.GetEmpresaId());
        // 1407 evitar conflictos con depostios disponibles agregarModel.TallerView = mecanicoService.ObtenerTaller(agregarModel.IdDeposito);
        // 1407 evitar conflictos con depostios disponibles agregarModel.DepositoJson = JsonConvert.SerializeObject(agregarModel.DepositoView);
        // 1407 evitar conflictos con depostios disponibles return View(agregarModel);
        // 1407 evitar conflictos con depostios disponibles }

        [HttpPost]
        public async Task<IActionResult> AgregarMecanico(MecanicoViewModel model)
        {
            try
            {
                // 1407 evitar conflictos con depostios disponibles model.TallerView = mecanicoService.ObtenerTaller(model.IdDeposito);

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
