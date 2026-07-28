using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    [Route("ModuloAdministrativo/Llantas")]
    public class LlantasController : Controller
    {
        private readonly ILlantaService llantaService;

        public LlantasController(ILlantaService llantaService)
        {
            this.llantaService = llantaService;
        }

        [HttpGet("")]
        [HttpGet("~/Llantas")]
        [HttpGet("~/Administrativo/ModuloAdministrativo/Llantas")]
        public async Task<IActionResult> Index()
        {
            var llantas = await llantaService.GetLlantas(User.GetEmpresaId());
            return View(llantas);
        }

        [HttpGet("~/llantas/crear")]
        public async Task<IActionResult> Crear()
        {
            var model = await llantaService.GetCrearViewModel(User.GetEmpresaId(), User.GetNombreEmpresa());
            return View(model);
        }

        [HttpPost("~/llantas/crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(LlantaCrearViewModel model)
        {
            try
            {
                var idEmpresa = User.GetEmpresaId();
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                if (!ModelState.IsValid)
                {
                    var recargar = await llantaService.GetCrearViewModel(idEmpresa, User.GetNombreEmpresa(), model);
                    return View(recargar);
                }

                await llantaService.CrearLlanta(model, idEmpresa, idUsuario);
                TempData["Mensaje"] = "Llanta registrada correctamente.";
                return RedirectToAction("Index", "Llantas");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var recargar = await llantaService.GetCrearViewModel(User.GetEmpresaId(), User.GetNombreEmpresa(), model);
                return View(recargar);
            }
        }

        [HttpGet("~/llantas/modelos-por-marca")]
        public async Task<IActionResult> GetModelosPorMarca(int idMarcaLlanta)
        {
            var modelos = await llantaService.GetModelosPorMarca(idMarcaLlanta);
            return Json(modelos);
        }

        [HttpGet("~/llantas/detalle-modelo")]
        public async Task<IActionResult> GetDetalleModelo(int idModeloLlanta)
        {
            var detalle = await llantaService.GetDetalleModelo(idModeloLlanta);
            return Json(detalle);
        }
    }
}
