using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    [Route("ModuloAdministrativo/LlantaAsignacion")]
    public class LlantaAsignacionController : Controller
    {
        private readonly ILlantaService llantaService;

        public LlantaAsignacionController(ILlantaService llantaService)
        {
            this.llantaService = llantaService;
        }

        [HttpGet("")]
        [HttpGet("~/llantaasignacion")]
        [HttpGet("~/llantasasignacion")]
        [HttpGet("~/Administrativo/ModuloAdministrativo/LlantaAsignacion")]
        public async Task<IActionResult> Index()
        {
            var model = new LlantaAsignacionViewModel
            {
                Vehiculos = await llantaService.GetVehiculosParaAsignacion(User.GetEmpresaId())
            };

            return View(model);
        }

        [HttpGet("Vehiculos")]
        [HttpGet("~/llantaasignacion/vehiculos")]
        [HttpGet("~/llantasasignacion/vehiculos")]
        public async Task<IActionResult> GetVehiculos()
        {
            try
            {
                var vehiculos = await llantaService.GetVehiculosParaAsignacion(User.GetEmpresaId());
                return Json(new { success = true, data = vehiculos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("LlantasDisponibles")]
        [HttpGet("~/llantaasignacion/llantas-disponibles")]
        [HttpGet("~/llantasasignacion/llantas-disponibles")]
        public async Task<IActionResult> GetLlantasDisponibles()
        {
            try
            {
                var llantas = await llantaService.GetLlantasDisponiblesParaInstalacion(User.GetEmpresaId());
                return Json(new { success = true, data = llantas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Instalar")]
        [HttpPost("~/llantaasignacion/instalar")]
        [HttpPost("~/llantasasignacion/instalar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Instalar(LlantaInstalacionViewModel model)
        {
            try
            {
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                await llantaService.InstalarLlanta(model, User.GetEmpresaId(), idUsuario);
                return Json(new { success = true, message = "Llanta instalada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
