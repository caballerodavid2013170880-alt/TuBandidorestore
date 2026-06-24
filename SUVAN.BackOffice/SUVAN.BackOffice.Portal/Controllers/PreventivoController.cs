using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper; // Espacio de nombres donde reside User.GetEmpresaId()
using SUVAN.BackOffice.Service.Logistica;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Portal.Controllers.Logistica
{
    /// <summary>
    /// Controlador encargado de procesar las interacciones de la interfaz de usuario para el catálogo de planes de mantenimiento preventivo.
    /// </summary>
    [Authorize]
    public class MantenimientoPreventivoController : Controller
    {
        private readonly ILogger<MantenimientoPreventivoController> _logger;
        private readonly IPreventivoService preventivoService;
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="MantenimientoPreventivoController"/> con inyección de dependencias estructurada.
        /// </summary>
        /// <param name="logger">Servicio de registros e incidencias del sistema de diagnóstico.</param>
        /// <param name="preventivoService">Capa de servicios lógicos asociados al preventivo.</param>
        public MantenimientoPreventivoController(ILogger<MantenimientoPreventivoController> logger, IPreventivoService preventivoService)
        {
            this._logger = logger;
            this.preventivoService = preventivoService;
        }

        /// <summary>
        /// Muestra la vista principal. Responde a la raíz del controlador.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var preventivos = await preventivoService.GetPreventivos(User.GetEmpresaId());
            return View(preventivos);
        }
        /// <summary>
        /// Muestra la vista principal con el listado general de mantenimientos preventivos configurados.
        /// </summary>
        /// <returns>Acción de resultado que renderiza la tabla de datos principal.</returns>
        /*//[HttpGet]
        public async Task<IActionResult> Preventivos()
        {
            var preventivos = await preventivoService.GetPreventivos(User.GetEmpresaId());
            return View(preventivos);
        }*/

        /// <summary>
        /// Maneja la petición GET para desplegar el formulario de alta o edición de un preventivo.
        /// </summary>
        /// <param name="id">Identificador único del registro (0 por defecto si es una inserción nueva).</param>
        /// <returns>Vista Razor del formulario instanciada con sus respectivos selectores maestros inicializados.</returns>
        //[HttpGet("Logistica/MantenimientoPreventivo/AgregarPreventivo/{id?}")]
        [HttpGet]
        public async Task<IActionResult> AgregarPreventivo(int id = 0)
        {
            var model = await preventivoService.GetPreventivoViewModel(User.GetEmpresaId(), id);
            return View(model);
        }

        /// <summary>
        /// Procesa la sumisión del formulario de captura mediante métodos POST seguros, validando consistencia interna.
        /// </summary>
        /// <param name="model">Datos del preventivo capturados y enlazados desde el cliente HTML.</param>
        /// <returns>Redirección al listado principal en caso de éxito, o recarga del formulario con alertas de validación.</returns>
        //[HttpPost("Logistica/MantenimientoPreventivo/AgregarPreventivo/{id?}")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AgregarPreventivo(PreventivoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var recargar = await preventivoService.GetPreventivoViewModel(User.GetEmpresaId(), model.Idpreventivo);
                    model.Plantas = recargar.Plantas;
                    model.Depositos = recargar.Depositos;
                    model.Marcas = recargar.Marcas;
                    model.Modelos = recargar.Modelos;
                    model.ManosObra = recargar.ManosObra;
                    return View(model);
                }

                int idEmpresa = User.GetEmpresaId();
                string claimUsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
                int idUsuario = Convert.ToInt32(claimUsuarioId);

                var result = await preventivoService.AgregarPreventivo(model, idEmpresa, idUsuario);

                if (result)
                {
                    TempData["Mensaje"] = model.Idpreventivo == 0
                        ? "Plan Preventivo registrado correctamente."
                        : "Plan Preventivo actualizado correctamente.";
                    return RedirectToAction("Index"); // <-- Redirigir a Index, ya no a Preventivos
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar guardar el plan.");
                ModelState.AddModelError(string.Empty, ex.Message);

                var recargar = await preventivoService.GetPreventivoViewModel(User.GetEmpresaId(), model.Idpreventivo);
                model.Plantas = recargar.Plantas;
                model.Marcas = recargar.Marcas;
                model.ManosObra = recargar.ManosObra;
                return View(model);
            }
        }

        /// <summary>
        /// Endpoint asíncrono en formato JSON para la actualización dinámica de depósitos subordinados a una planta.
        /// </summary>
        /// <param name="idPlanta">Identificador de la planta padre.</param>
        /// <returns>Arreglo serializado de depósitos válidos.</returns>
        //[HttpGet("GetDepositosPorPlanta")]
        [HttpGet]
        public async Task<IActionResult> GetDepositosPorPlanta(int idPlanta)
        {
            var depositos = await preventivoService.GetDepositosPorPlanta(User.GetEmpresaId(), idPlanta);
            return Json(depositos);
        }

        /// <summary>
        /// Endpoint asíncrono en formato JSON para la actualización dinámica de modelos subordinados a una marca.
        /// </summary>
        /// <param name="idMarca">Identificador de la marca padre.</param>
        /// <returns>Arreglo serializado de modelos válidos.</returns>
        //[HttpGet("GetModelosPorMarca")]
        [HttpGet]
        public async Task<IActionResult> GetModelosPorMarca(short idMarca)
        {
            var modelos = await preventivoService.GetModelosPorMarca(idMarca);
            return Json(modelos);
        }

        // ===================     SP       ====================
        /// <summary>
        /// Estructura auxiliar para recibir datos JSON desde Fetch
        /// </summary>
        public class GenerarPreventivoRequest
        {
            public int IdPreventivo { get; set; }
            public int IdManoObra { get; set; }
            public DateTime FechaPrev { get; set; }
        }

        /// <summary>
        /// Endpoint asíncrono que recibe la orden de procesar el Stored Procedure para generar preventivos.
        /// </summary>
        [HttpPost("GenerarDetalles")]
        public async Task<IActionResult> GenerarDetalles([FromBody] GenerarPreventivoRequest request)
        {
            try
            {
                if (request == null || request.IdPreventivo <= 0 || request.IdManoObra <= 0 || request.FechaPrev == DateTime.MinValue) 
                    return Json(new { success = false, message = "Datos inválidos. Asegúrese de seleccionar una Mano de Obra." });

                int idEmpresa = User.GetEmpresaId();
                string claimUsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0";
                int idUsuario = Convert.ToInt32(claimUsuarioId);

                var result = await preventivoService.GenerarDetallePreventivoAsync(request.IdPreventivo, request.IdManoObra, request.FechaPrev, idEmpresa, idUsuario);
                return Json(new { success = true, message = "Los preventivos han sido generados exitosamente en el Detalle." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al ejecutar generación de preventivos para el ID {request?.IdPreventivo}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Muestra la vista con el resumen general del plan y la tabla masiva de det_prev.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DetalleGeneral(int id)
        {
            try
            {
                if (id <= 0) return RedirectToAction("Index");
                var model = await preventivoService.GetDetalleGeneralAsync(User.GetEmpresaId(), id);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar detalle general para el ID {id}");
                TempData["MensajeError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Muestra la vista con el desglose unitario por vehículo coincidente (det_prev_mo).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DetallePreventivoMO(int id)
        {
            try
            {
                if (id <= 0) return RedirectToAction("Index");
                ViewBag.IdPreventivo = id;
                var model = await preventivoService.GetDetalleVehiculosAsync(User.GetEmpresaId(), id);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar detalle de vehículos para el ID {id}");
                TempData["MensajeError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}