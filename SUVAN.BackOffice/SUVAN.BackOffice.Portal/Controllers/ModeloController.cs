using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.CausaMantenimientoViewModel;
using SUVAN.BackOffice.Portal.Models;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class ModeloController : Controller
    {
        private readonly ILogger<ModeloController> _logger;
        private readonly IModeloService _modeloService;

        public ModeloController(ILogger<ModeloController> logger, IModeloService modeloService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _modeloService = modeloService ?? throw new ArgumentNullException(nameof(modeloService));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var modelos = await _modeloService.GetModelos();
                return View(modelos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de modelos.");
                return View("Error", new ErrorViewModel { Message = "No se pudo obtener los modelos." });
            }
        }

        public async Task<IActionResult> AgregarModelo(int id)
        {
            try
            {
                var agregarModel = await _modeloService.GetModeloViewModel(id);
                return View(agregarModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el formulario de agregar modelo.");
                return View("Error", new ErrorViewModel { Message = "No se pudo cargar el modelo." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarModelo(Database.Entities.Modelo model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en AgregarModelo.");
                return View(model);
            }

            try
            {
                var result = await _modeloService.AgregarModelo(model);
                if (result)
                {
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("No se pudo agregar el modelo.");
                ModelState.AddModelError(string.Empty, "No se pudo agregar el modelo.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el modelo.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarModelo([FromBody] Database.Entities.Modelo model)
        {
            if (model?.IdModelo == null || model.IdModelo <= 0)
            {
                _logger.LogWarning("ID inválido para eliminar modelo.");
                return BadRequest(new { success = false, message = "ID de modelo inválido." });
            }

            try
            {
                await _modeloService.EliminarModelo(model.IdModelo);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el modelo con ID {model.IdModelo}.");
                return StatusCode(500, new { success = false, message = "Error interno al eliminar el modelo." });
            }
        }
    }
}