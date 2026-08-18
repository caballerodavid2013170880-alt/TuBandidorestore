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

        [HttpGet("~/llantas/editar/{id}")]
        public async Task<IActionResult> Editar(ulong id)
        {
            var model = await llantaService.GetEditarViewModel(id, User.GetEmpresaId(), User.GetNombreEmpresa());
            if (model == null)
            {
                TempData["Mensaje"] = "No se encontró la llanta solicitada.";
                return RedirectToAction("Index", "Llantas");
            }

            return View("Crear", model);
        }

        [HttpPost("~/llantas/editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ulong id, LlantaCrearViewModel model)
        {
            try
            {
                var idEmpresa = User.GetEmpresaId();
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                model.IdLlanta = id;

                if (!ModelState.IsValid)
                {
                    var recargar = await llantaService.GetCrearViewModel(idEmpresa, User.GetNombreEmpresa(), model);
                    return View("Crear", recargar);
                }

                await llantaService.ActualizarLlanta(model, idEmpresa, idUsuario);
                TempData["Mensaje"] = "Llanta actualizada correctamente.";
                return RedirectToAction("Index", "Llantas");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.IdLlanta = id;
                var recargar = await llantaService.GetCrearViewModel(User.GetEmpresaId(), User.GetNombreEmpresa(), model);
                return View("Crear", recargar);
            }
        }

        [HttpPost("~/llantas/eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(ulong id)
        {
            try
            {
                var idEmpresa = User.GetEmpresaId();
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaService.EliminarLlanta(id, idEmpresa, idUsuario);
                return Json(new { success = true, message = "Llanta eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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

        [HttpGet("~/llantas/vehiculo/{idVehiculo}/configuracion")]
        public async Task<IActionResult> GetConfiguracionVehiculo(int idVehiculo)
        {
            try
            {
                var configuracion = await llantaService.GetConfiguracionVehiculoLlantas(idVehiculo, User.GetEmpresaId());
                return Json(new { success = true, data = configuracion });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
