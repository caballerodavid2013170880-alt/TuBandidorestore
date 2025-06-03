using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.ViewModel.Logistica.CausaMantenimientoViewModel;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class MarcaController : Controller
    {
        private readonly ILogger<MarcaController> _logger;
        private readonly IMarcaService _marcaService;


        public MarcaController(ILogger<MarcaController> logger, IMarcaService marcaService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _marcaService = marcaService ?? throw new ArgumentNullException(nameof(marcaService));
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var marcas = await _marcaService.GetMarcas();
                return View(marcas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de marcas.");
                return View("Error", new { message = "No se pudo obtener las marcas." });
            }
        }


        public async Task<IActionResult> AgregarMarca(int id)
        {
            try
            {
                var agregarModel = await _marcaService.GetMarcaViewModel(id);
                return View(agregarModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el formulario de agregar marca.");
                return View("Error", new { message = "No se pudo cargar la marca." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMarca(Database.Entities.Marca model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en AgregarMarca.");
                return View(model);
            }

            try
            {
                var result = await _marcaService.AgregarMarca(model);
                if (result)
                {
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("No se pudo agregar la marca.");
                ModelState.AddModelError(string.Empty, "No se pudo agregar la marca.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar la marca.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarMarca([FromBody] Database.Entities.Marca model)
        {
            if (model?.IdMarca == null || model.IdMarca <= 0)
            {
                _logger.LogWarning("ID inválido para eliminar marca.");
                return BadRequest(new { success = false, message = "ID de marca inválido." });
            }

            try
            {
                await _marcaService.EliminarMarca(model.IdMarca);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la marca con ID {model.IdMarca}.");
                return StatusCode(500, new { success = false, message = "Error interno al eliminar la marca." });
            }
        }

    }
}