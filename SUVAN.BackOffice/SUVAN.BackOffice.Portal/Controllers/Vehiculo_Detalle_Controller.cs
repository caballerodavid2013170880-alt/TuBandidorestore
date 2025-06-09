using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Logistica;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using SUVAN.BackOffice.Portal.Models; // Asegúrate de que esta es la ubicación correcta de ErrorViewModel

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class VehiculoDetalleController : Controller
    {
        private readonly ILogger<VehiculoDetalleController> _logger;
        private readonly IVehiculoDetalleService _vehiculoDetalleService;

        public VehiculoDetalleController(ILogger<VehiculoDetalleController> logger, IVehiculoDetalleService vehiculoDetalleService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vehiculoDetalleService = vehiculoDetalleService ?? throw new ArgumentNullException(nameof(vehiculoDetalleService));
        }

        // 📌 Listado de vehículos
        public async Task<IActionResult> Index()
        {
            try
            {
                var vehiculos = await _vehiculoDetalleService.GetVehiculos();
                return View(vehiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de vehículos.");
                return View("Error", new ErrorViewModel { Message = "No se pudo obtener los vehículos." });
            }
        }

        // Formulario para agregar vehículo
        public async Task<IActionResult> AgregarVehiculo(int id)
        {
            try
            {
                var agregarModel = await _vehiculoDetalleService.GetVehiculoViewModel(id);
                return View(agregarModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el formulario de agregar vehículo.");
                return View("Error", new ErrorViewModel { Message = "No se pudo cargar el vehículo." });
            }
        }

        // Agregar un vehículo
        [HttpPost]
        public async Task<IActionResult> AgregarVehiculo(Database.Entities.VehiculoDetalle model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en AgregarVehiculo.");
                return View(model);
            }

            try
            {
                var result = await _vehiculoDetalleService.AgregarVehiculo(model);
                if (result)
                {
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("No se pudo agregar el vehículo.");
                ModelState.AddModelError(string.Empty, "No se pudo agregar el vehículo.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el vehículo.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // Eliminar un vehículo
        [HttpPost]
        public async Task<IActionResult> EliminarVehiculo([FromBody] Database.Entities.VehiculoDetalle model)
        {
            if (model?.IdVehicDetalle == null || model.IdVehicDetalle <= 0)
            {
                _logger.LogWarning("ID inválido para eliminar vehículo.");
                return BadRequest(new { success = false, message = "ID de vehículo inválido." });
            }

            try
            {
                await _vehiculoDetalleService.EliminarVehiculo(model.IdVehicDetalle);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el vehículo con ID {model.IdVehicDetalle}.");
                return StatusCode(500, new { success = false, message = "Error interno al eliminar el vehículo." });
            }
        }
    }
}