using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    [Route("ModuloAdministrativo/LlantasModelo")]
    public class LlantasModeloController : Controller
    {
        private readonly ILlantaModeloService llantaModeloService;

        public LlantasModeloController(ILlantaModeloService llantaModeloService)
        {
            this.llantaModeloService = llantaModeloService;
        }

        [HttpGet("")]
        [HttpGet("~/llantasmodelo")]
        [HttpGet("~/Administrativo/ModuloAdministrativo/LlantasModelo")]
        public async Task<IActionResult> Index()
        {
            var modelos = await llantaModeloService.GetModelos();
            return View(modelos);
        }

        [HttpGet("Crear")]
        [HttpGet("~/llantasmodelo/crear")]
        public async Task<IActionResult> Crear()
        {
            var model = await llantaModeloService.GetCrearViewModel();
            return View(model);
        }

        [HttpPost("Crear")]
        [HttpPost("~/llantasmodelo/crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(LlantaModeloViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model = await llantaModeloService.GetCrearViewModel(model);
                    return View(model);
                }

                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaModeloService.CrearModelo(model, idUsuario);
                TempData["Mensaje"] = "Modelo de llanta registrado correctamente.";
                return RedirectToAction("Index", "LlantasModelo");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model = await llantaModeloService.GetCrearViewModel(model);
                return View(model);
            }
        }

        [HttpGet("Editar/{id}")]
        [HttpGet("~/llantasmodelo/editar/{id}")]
        public async Task<IActionResult> Editar(uint id)
        {
            var model = await llantaModeloService.GetModeloViewModel(id);

            if (model.IdModeloLlanta == 0)
            {
                TempData["Mensaje"] = "No se encontró el modelo de llanta solicitado.";
                return RedirectToAction("Index", "LlantasModelo");
            }

            return View("Crear", model);
        }

        [HttpPost("Editar/{id}")]
        [HttpPost("~/llantasmodelo/editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(uint id, LlantaModeloViewModel model)
        {
            try
            {
                model.IdModeloLlanta = id;

                if (!ModelState.IsValid)
                {
                    model = await llantaModeloService.GetCrearViewModel(model);
                    return View("Crear", model);
                }

                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaModeloService.ActualizarModelo(model, idUsuario);
                TempData["Mensaje"] = "Modelo de llanta actualizado correctamente.";
                return RedirectToAction("Index", "LlantasModelo");
            }
            catch (Exception ex)
            {
                model.IdModeloLlanta = id;
                ModelState.AddModelError(string.Empty, ex.Message);
                model = await llantaModeloService.GetCrearViewModel(model);
                return View("Crear", model);
            }
        }

        [HttpPost("Eliminar")]
        [HttpPost("~/llantasmodelo/eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(uint id)
        {
            try
            {
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaModeloService.EliminarModelo(id, idUsuario);
                return Json(new { success = true, message = "Modelo de llanta eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
