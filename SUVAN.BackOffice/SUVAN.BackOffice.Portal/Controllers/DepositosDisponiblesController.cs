using Microsoft.AspNetCore.Authorization;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.MensajeriaService;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class DepositosDisponiblesController : Controller
    {
        private readonly ILogger<DepositosDisponiblesController> _logger;
        private readonly IDepositosDisponibles depositosDisponibles;

        public DepositosDisponiblesController(ILogger<DepositosDisponiblesController> logger,
        IDepositosDisponibles depositosDisponibles)

        {
            _logger = logger;
            this.depositosDisponibles = depositosDisponibles;

        }
        public async Task<IActionResult> Index()
        {
            var depositos = await depositosDisponibles.GetDepositos(User.GetEmpresaId());
            return View(depositos);
        }
        public async Task<IActionResult> AgregarDeposito(int id)
        {
            var agregarModel = await depositosDisponibles.GetDepositoViewModel(id);
            agregarModel.ZonasView = depositosDisponibles.ObtenerZona(User.GetEmpresaId());
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDeposito(DepositosDisponiblesViewModel model, int IdEmpresa)
        {
            try
            {
                IdEmpresa = User.GetEmpresaId();

                model.ZonasView = depositosDisponibles.ObtenerZona(IdEmpresa);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await depositosDisponibles.AgregarDeposito(model, IdEmpresa);

                if (result)
                {
                    return RedirectToAction("Index", "DepositosDisponibles");
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
        public async Task<IActionResult> EliminarDeposito([FromBody] DepositosDisponiblesViewModel model)
        {
            try
            {
                await depositosDisponibles.EliminarDeposito(model.DepositoId);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
