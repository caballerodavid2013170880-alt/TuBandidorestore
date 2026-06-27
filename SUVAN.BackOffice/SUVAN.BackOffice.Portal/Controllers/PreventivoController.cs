using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Portal.Controllers.Logistica
{
    [Authorize]
    public class MantenimientoPreventivoController : Controller
    {
        private readonly ILogger<MantenimientoPreventivoController> _logger;
        private readonly IPreventivoService preventivoService;

        public MantenimientoPreventivoController(ILogger<MantenimientoPreventivoController> logger, IPreventivoService preventivoService)
        {
            _logger = logger;
            this.preventivoService = preventivoService;
        }

        // 1. VISTA: MANTENIMIENTOS (Principal)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var preventivos = await preventivoService.GetPreventivos(User.GetEmpresaId());
            return View(preventivos);
        }

        // 2. VISTA: DETALLE DE PREVENTIVOS (COmbinada)
        [HttpGet]
        public async Task<IActionResult> DetallePreventivos(int id = 0)
        {
            ViewBag.IdPreventivoSeleccionado = id;
            ViewBag.ListaPreventivos = await preventivoService.GetDropdownPreventivos(User.GetEmpresaId());
            return View();
        }

        // 3. AGREGAR / EDITAR
        [HttpGet]
        public async Task<IActionResult> AgregarPreventivo(int id = 0)
        {
            var model = await preventivoService.GetPreventivoViewModel(User.GetEmpresaId(), id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPreventivoAjax(PreventivoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return Json(new { success = false, message = "Datos incompletos o inválidos." });

                int idEmpresa = User.GetEmpresaId();
                int idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                int newId = await preventivoService.AgregarPreventivoAjax(model, idEmpresa, idUsuario);

                return Json(new { success = true, idPreventivo = newId, message = "Plan guardado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el preventivo.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerarDetallesAjax([FromBody] PreventivoViewModel model)
        {
            try
            {
                if (model.Idpreventivo <= 0 || model.IdManoObra <= 0) return Json(new { success = false, message = "Faltan datos para generar." });
                int idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                await preventivoService.GenerarDetallePreventivoAsync(model.Idpreventivo, model.IdManoObra, model.FechaPrev.Value, User.GetEmpresaId(), idUsuario);
                return Json(new { success = true, message = "Preventivos generados exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // MODAL Y TABLAS AJAX
        [HttpGet]
        public async Task<IActionResult> GetModalDetalle(int id)
        {
            var model = await preventivoService.GetDetalleGeneralAsync(User.GetEmpresaId(), id);
            ViewBag.Vehiculos = await preventivoService.GetDetalleVehiculosAsync(User.GetEmpresaId(), id);
            return PartialView("_DetalleModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDatosDetalleView(int id)
        {
            var model = await preventivoService.GetDetalleGeneralAsync(User.GetEmpresaId(), id);
            var vehiculos = await preventivoService.GetDetalleVehiculosAsync(User.GetEmpresaId(), id);
            return Json(new { success = true, general = model, vehiculos = vehiculos });
        }

        public async Task<IActionResult> DetalleGeneral(int id)
        {
            var model = await preventivoService.GetDetalleGeneralAsync(User.GetEmpresaId(), id);
            // Carga los vehículos asociados a este preventivo específico
            ViewBag.Vehiculos = await preventivoService.GetDetalleVehiculosAsync(User.GetEmpresaId(), id);
            return View(model);
        }

        // ENDPOINTS CASCADA
        [HttpGet] public async Task<IActionResult> GetPlantasPorRegion(int id) => Json(await preventivoService.GetPlantasPorRegion(id));
        [HttpGet] public async Task<IActionResult> GetZonasPorPlanta(int id) => Json(await preventivoService.GetZonasPorPlanta(id));
        [HttpGet] public async Task<IActionResult> GetDepositosPorZona(int id) => Json(await preventivoService.GetDepositosPorZona(id));
        [HttpGet] public async Task<IActionResult> GetModelosPorMarca(short id) => Json(await preventivoService.GetModelosPorMarca(id));
    }
}