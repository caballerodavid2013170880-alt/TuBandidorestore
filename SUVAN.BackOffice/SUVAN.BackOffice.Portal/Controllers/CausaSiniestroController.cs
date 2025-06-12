using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Models;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class CausaSiniestroController : Controller
    {
        private readonly ILogger<CausaSiniestroController> _logger;
        private readonly ICausaSiniestroService _causaSiniestroService;

        public CausaSiniestroController(ILogger<CausaSiniestroController> logger, ICausaSiniestroService causaSiniestroService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _causaSiniestroService = causaSiniestroService ?? throw new ArgumentNullException(nameof(causaSiniestroService));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var causasSiniestro = await _causaSiniestroService.GetCausaSiniestro();
                return View(causasSiniestro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de causas de siniestro.");
                return View("Error", new ErrorViewModel { Message = "No se pudo obtener las causas de siniestro." });
            }
        }

        public async Task<IActionResult> AgregarCausaSiniestro(int id)
        {
            try
            {
                var agregarModel = await _causaSiniestroService.GetCausaSiniestroViewModel(id);
                return View(agregarModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el formulario de agregar causa de siniestro.");
                return View("Error", new ErrorViewModel { Message = "No se pudo cargar la causa de siniestro." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCausaSiniestro(CausaSiniestroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en AgregarCausaSiniestro.");
                return View(model);
            }

            try
            {
                var result = await _causaSiniestroService.AgregarCausaSiniestro(model);
                if (result)
                {
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("No se pudo agregar la causa de siniestro.");
                ModelState.AddModelError(string.Empty, "No se pudo agregar la causa de siniestro.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar la causa de siniestro.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCausaSiniestro([FromBody] CausaSiniestroViewModel model)
        {
            if (model?.Id_causa_siniestro == null || model.Id_causa_siniestro <= 0)
            {
                _logger.LogWarning("ID inválido para eliminar causa de siniestro.");
                return BadRequest(new { success = false, message = "ID de causa de siniestro inválido." });
            }

            try
            {
                await _causaSiniestroService.EliminarCausaSiniestro(model.Id_causa_siniestro);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la causa de siniestro con ID {model.Id_causa_siniestro}.");
                return StatusCode(500, new { success = false, message = "Error interno al eliminar la causa de siniestro." });
            }
        }
    }
}