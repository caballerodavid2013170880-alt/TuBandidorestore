using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;
using SUVAN.BackOffice.Portal.Helper;

namespace SUVAN.BackOffice.Portal.Controllers.Logistica
{
    [Authorize]
    //[Route("ManoObra")]
    public class ManoObraController : Controller
    {
        private readonly ILogger<ManoObraController> _logger;
        private readonly IManoObraService _manoObraService;
        public ManoObraController(ILogger<ManoObraController> logger, IManoObraService manoObraService)
        {
            _logger = logger;
            _manoObraService = manoObraService;
        }
        private int GetUserId()
        {
            return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        private (int idEmpresa, int idUsuario, int? idRegion, int? idPlanta, int? idZona, int? idDeposito) GetUserContext()
        {
            int idEmpresa = User.GetEmpresaId();
            int idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Reemplazar estas asignaciones cuando los claims de jerarquía estén implementados
            int? idRegion = null;
            int? idPlanta = null;
            int? idZona = null;
            int? idDeposito = null;

            return (idEmpresa, idUsuario, idRegion, idPlanta, idZona, idDeposito);
        }

        //[HttpGet]
        //[HttpGet("")]
        //[HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var model = await _manoObraService.GetManoObras();
            return View(model);
        }
        [HttpGet("AgregarManoObra")]
        public async Task<IActionResult> AgregarManoObra(int id = 0)
        {
            if (id == 0)
            {
                return View(new ManoObraViewModel());
            }
            var model = await _manoObraService.GetManoObra(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost] // HttpPost simple para evitar el error de rutas (InvalidOperationException)
        public async Task<IActionResult> AgregarManoObraAjax([FromBody] ManoObraViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = errors });
                }

                int idUsuario = GetUserId();
                int newId = await _manoObraService.GuardarManoObraAsync(model, idUsuario);

                return Json(new { success = true, idManoObra = newId, message = "Mano de obra guardada correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar mano de obra.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("/DetalleManoObras")]
        public async Task<IActionResult> DetalleManoObras()
        {
            // Obtener la validación del contexto
            var usr = GetUserContext();

            // Enviar los datos del contexto a la vista para rellenar los data-attributes (data-context-region, etc.)
            ViewBag.IdRegion = usr.idRegion;
            ViewBag.IdPlanta = usr.idPlanta;
            ViewBag.IdZona = usr.idZona;
            ViewBag.IdDeposito = usr.idDeposito;

            // Obtener el modelo general de Mano de Obra
            // Nota: Si el servicio se actualiza posteriormente para filtrar por empresa o región,
            // se le pasarían los parámetros de la variable 'usr' aquí.
            var model = await _manoObraService.GetManoObras();

            // Retornar la vista explícitamente por si el nombre de la ruta difiere del archivo
            return View("DetalleManoObras", model);
        }
        //[HttpGet("GetModalDetalle")]
        public async Task<IActionResult> GetModalDetalle(int id)
        {
            var model = await _manoObraService.GetActividadesPorManoObra(id);
            var manoObra = await _manoObraService.GetManoObra(id);
            ViewBag.Servicio = manoObra?.DescripcionManoobra;
            return PartialView("_DetalleModal", model);
        }
    }
}