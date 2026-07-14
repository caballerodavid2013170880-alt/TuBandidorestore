using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Service.Administrativo;
using System.Security.Claims;
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
        //[HttpPost("AgregarManoObraAjax")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AgregarManoObraAjax(ManoObraViewModel model)
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
        //[HttpGet("DetalleManoObras")]
        public async Task<IActionResult> DetalleManoObras()
        {
            var model = await _manoObraService.GetTodasActividades();
            return View(model);
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