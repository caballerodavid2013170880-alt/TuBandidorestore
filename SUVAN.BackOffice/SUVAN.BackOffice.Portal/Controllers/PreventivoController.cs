using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
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

        /// <summary>
        /// Obtiene variables de contexto de sesión del usuario para validación de datos.
        /// </summary>
        private (int idEmpresa, int idUsuario, int? idRegion, int? idPlanta, int? idZona, int? idDeposito) GetUserContext()
        {
            int idEmpresa = User.GetEmpresaId();
            int idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Reemplazar estas asignaciones nulas cuando los claims de jerarquía estén implementados
            int? idRegion = null;
            int? idPlanta = null;
            int? idZona = null;
            int? idDeposito = null;

            return (idEmpresa, idUsuario, idRegion, idPlanta, idZona, idDeposito);
        }
        //Mantenimiento Preventivo y Detalle de Preventivos
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usr = GetUserContext();
            var preventivos = await preventivoService.GetPreventivos(usr.idEmpresa, usr.idRegion, usr.idPlanta, usr.idZona, usr.idDeposito);
            return View(preventivos);
        }

        [HttpGet]
        public async Task<IActionResult> DetallePreventivos(int id = 0)
        {
            var usr = GetUserContext();
            ViewBag.IdPreventivoSeleccionado = id;
            ViewBag.ListaPreventivos = await preventivoService.GetDropdownPreventivos(usr.idEmpresa, usr.idRegion, usr.idPlanta);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AgregarPreventivo(int id = 0)
        {
            var usr = GetUserContext();
            var model = await preventivoService.GetPreventivoViewModel(usr.idEmpresa, id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPreventivoAjax(PreventivoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return Json(new { success = false, message = "Datos incompletos o inválidos." });

                var usr = GetUserContext();
                int newId = await preventivoService.AgregarPreventivoAjax(model, usr.idEmpresa, usr.idUsuario);

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
                var usr = GetUserContext();
                await preventivoService.GenerarDetallePreventivoAsync(model.Idpreventivo, model.IdManoObra, model.FechaPrev.Value, usr.idEmpresa, usr.idUsuario);
                return Json(new { success = true, message = "Preventivos generados exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetModalDetalle(int id)
        {
            var usr = GetUserContext();
            var model = await preventivoService.GetDetalleGeneralAsync(usr.idEmpresa, id, usr.idRegion, usr.idPlanta);
            ViewBag.Vehiculos = await preventivoService.GetDetalleVehiculosAsync(usr.idEmpresa, id, usr.idRegion);
            return PartialView("_DetalleModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDatosDetalleView(int id)
        {
            var usr = GetUserContext();
            var model = await preventivoService.GetDetalleGeneralAsync(usr.idEmpresa, id, usr.idRegion, usr.idPlanta);
            var vehiculos = await preventivoService.GetDetalleVehiculosAsync(usr.idEmpresa, id, usr.idRegion);
            return Json(new { success = true, general = model, vehiculos = vehiculos });
        }

        //Consulta de Mantto Preventivos
        [HttpGet("/ConsultaPrev")]
        public async Task<IActionResult> ConsultaPreventivos()
        {
            var usr = GetUserContext();
            // Carga catálogos para los combos de filtros
            var model = await preventivoService.GetPreventivoViewModel(usr.idEmpresa, 0);
            // Carga todos los planes para alimentar la tabla
            ViewBag.ListaPreventivos = await preventivoService.GetPreventivos(usr.idEmpresa, usr.idRegion, usr.idPlanta, usr.idZona, usr.idDeposito);
            return View(model);
        }

        // ENDPOINTS CASCADA
        [HttpGet] public async Task<IActionResult> GetPlantasPorRegion(int id) => Json(await preventivoService.GetPlantasPorRegion(id));
        [HttpGet] public async Task<IActionResult> GetZonasPorPlanta(int id) => Json(await preventivoService.GetZonasPorPlanta(id));
        [HttpGet] public async Task<IActionResult> GetDepositosPorZona(int id) => Json(await preventivoService.GetDepositosPorZona(id));
        [HttpGet] public async Task<IActionResult> GetModelosPorMarca(short id) => Json(await preventivoService.GetModelosPorMarca(id));
    }
}