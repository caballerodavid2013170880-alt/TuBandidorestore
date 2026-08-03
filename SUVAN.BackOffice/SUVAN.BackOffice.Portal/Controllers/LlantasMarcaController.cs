using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    [Route("ModuloAdministrativo/LlantasMarca")]
    public class LlantasMarcaController : Controller
    {
        private readonly ILlantaMarcaService llantaMarcaService;

        public LlantasMarcaController(ILlantaMarcaService llantaMarcaService)
        {
            this.llantaMarcaService = llantaMarcaService;
        }

        [HttpGet("")]
        [HttpGet("~/llantasmarca")]
        [HttpGet("~/Administrativo/ModuloAdministrativo/LlantasMarca")]
        public async Task<IActionResult> Index()
        {
            var marcas = await llantaMarcaService.GetMarcas();
            return View(marcas);
        }

        [HttpGet("Crear")]
        [HttpGet("~/llantasmarca/crear")]
        public IActionResult Crear()
        {
            return View(new LlantaMarcaViewModel());
        }

        [HttpPost("Crear")]
        [HttpPost("~/llantasmarca/crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(LlantaMarcaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaMarcaService.CrearMarca(model, idUsuario);
                TempData["Mensaje"] = "Marca de llanta registrada correctamente.";
                return RedirectToAction("Index", "LlantasMarca");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet("Editar/{id}")]
        [HttpGet("~/llantasmarca/editar/{id}")]
        public async Task<IActionResult> Editar(uint id)
        {
            var model = await llantaMarcaService.GetMarcaViewModel(id);

            if (model.IdMarcaLlanta == 0)
            {
                TempData["Mensaje"] = "No se encontró la marca de llanta solicitada.";
                return RedirectToAction("Index", "LlantasMarca");
            }

            return View("Crear", model);
        }

        [HttpPost("Editar/{id}")]
        [HttpPost("~/llantasmarca/editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(uint id, LlantaMarcaViewModel model)
        {
            try
            {
                model.IdMarcaLlanta = id;

                if (!ModelState.IsValid)
                {
                    return View("Crear", model);
                }

                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaMarcaService.ActualizarMarca(model, idUsuario);
                TempData["Mensaje"] = "Marca de llanta actualizada correctamente.";
                return RedirectToAction("Index", "LlantasMarca");
            }
            catch (Exception ex)
            {
                model.IdMarcaLlanta = id;
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Crear", model);
            }
        }

        [HttpPost("Eliminar")]
        [HttpPost("~/llantasmarca/eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(uint id)
        {
            try
            {
                var idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                await llantaMarcaService.EliminarMarca(id, idUsuario);
                return Json(new { success = true, message = "Marca de llanta eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
