using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    [Route("Llantas/Inspeccion")]
    public class LlantaInspeccionController : Controller
    {
        private readonly ILlantaInspeccionService llantaInspeccionService;

        public LlantaInspeccionController(ILlantaInspeccionService llantaInspeccionService)
        {
            this.llantaInspeccionService = llantaInspeccionService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = await llantaInspeccionService.GetViewModel(User.GetEmpresaId());
            return View(model);
        }

        [HttpGet("Vehiculo/{idVehiculo:int}/Configuracion")]
        public async Task<IActionResult> GetConfiguracionVehiculo(int idVehiculo)
        {
            try
            {
                var configuracion = await llantaInspeccionService.GetConfiguracionVehiculo(idVehiculo, User.GetEmpresaId());
                return Json(new { success = true, data = configuracion });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Guardar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(LlantaInspeccionGuardarViewModel model)
        {
            try
            {
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                await llantaInspeccionService.GuardarInspeccion(model, User.GetEmpresaId(), idUsuario);
                return Json(new { success = true, message = "Inspección registrada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
