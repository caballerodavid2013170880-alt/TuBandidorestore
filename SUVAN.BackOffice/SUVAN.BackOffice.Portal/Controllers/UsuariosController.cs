using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class UsuariosController : Controller
  {
    private readonly ILogger<PerfilesController> logger;
    private readonly IAdminService adminService;
    private readonly IUsuarioJerarquiaService usuarioJerarquiaService;

        public UsuariosController(
            ILogger<PerfilesController> logger, 
            IAdminService adminService, 
            IUsuarioJerarquiaService usuarioJerarquiaService)
    {
      this.logger = logger;
      this.adminService = adminService;
            this.usuarioJerarquiaService = usuarioJerarquiaService;
    }
    public async Task<IActionResult> Index()
    {
      var admins = await adminService.GetAdmins();
      return View(admins);
    }

    public async Task<IActionResult> Agregar(int id)
    {
      var agregarModel = await adminService.GetAdminViewModel(id);
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarUsuarioViewModel model)
    {
      var returnModel = await adminService.GetAdminViewModel(0);
      try
      {
        returnModel.Nombre = model.Nombre;
        returnModel.Email = model.Email;
        returnModel.PerfilId = model.PerfilId;
        returnModel.AdminId = model.AdminId;

        if (!ModelState.IsValid)
        {
          return View(returnModel);
        }

        var perfilId = await adminService.AgregarUsuario(model);

        if (perfilId > 0)
        {
          return RedirectToAction("Index", "Usuarios");
        }

        return View(returnModel);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);

        return View(returnModel);
      }

    }

    [HttpPost]
    public async Task<IActionResult> Eliminar([FromBody] DeleteUsuarioViewModel model)
    {
      try
      {
        var result = await adminService.EliminarAdmin(model.UserId);

        if (result)
        {
          return Ok(result);
        }

        return BadRequest(result);

      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

        // =========================================================================
        // Endpoints AJAX para cascada jerárquica por Empresa asignada al Usuario
        // =========================================================================

        [HttpGet]
        public async Task<IActionResult> GetRegionesPorEmpresa(int idEmpresa)
        {
            var regiones = await usuarioJerarquiaService.GetRegionesPorEmpresa(idEmpresa);
            return Json(regiones);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlantasPorRegion(int idEmpresa, int idRegion)
        {
            var plantas = await usuarioJerarquiaService.GetPlantasPorRegion(idEmpresa, idRegion);
            return Json(plantas);
        }

        [HttpGet]
        public async Task<IActionResult> GetZonasPorPlanta(int idEmpresa, int idRegion, int idPlanta)
        {
            var zonas = await usuarioJerarquiaService.GetZonasPorPlanta(idEmpresa, idRegion, idPlanta);
            return Json(zonas);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepositosPorZona(int idEmpresa, int idRegion, int idPlanta, int idZona)
        {
            var depositos = await usuarioJerarquiaService.GetDepositosPorZona(idEmpresa, idRegion, idPlanta, idZona);
            return Json(depositos);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeptosPorDeposito(int idEmpresa, int idRegion, int idPlanta, int idZona, int idDeposito)
        {
            var deptos = await usuarioJerarquiaService.GetDeptosPorDeposito(idEmpresa, idRegion, idPlanta, idZona, idDeposito);
            return Json(deptos);
        }

        [HttpGet]
        public async Task<IActionResult> GetJerarquiaUsuario(int adminId, int idEmpresa)
        {
            if (adminId <= 0 || idEmpresa <= 0)
            {
                return Json(new { success = false });
            }

            var jerarquia = await usuarioJerarquiaService.GetJerarquiaUsuario("Admin", adminId, idEmpresa);
            if (jerarquia != null)
            {
                return Json(new
                {
                    success = true,
                    idRegion = jerarquia.IdRegion,
                    idPlanta = jerarquia.IdPlanta,
                    idZona = jerarquia.IdZona,
                    idDeposito = jerarquia.IdDeposito,
                    idDepto = jerarquia.IdDepto
                });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetJerarquiasUsuario(int adminId, int idEmpresa)
        {
            if (adminId <= 0 || idEmpresa <= 0)
            {
                return Json(new { success = false, items = new List<UsuarioJerarquiaItemViewModel>() });
            }

            var jerarquias = await usuarioJerarquiaService.GetJerarquiasUsuario("Admin", adminId, idEmpresa);
            var items = jerarquias.Select(j => new UsuarioJerarquiaItemViewModel
            {
                idUsuarioJerarquia = j.IdUsuarioJerarquia,
                empresaId = j.IdEmpresa,
                regionId = j.IdRegion,
                regionNombre = j.IdRegionNavigation?.NombreRegion ?? string.Empty,
                plantaId = j.IdPlanta,
                plantaNombre = j.IdPlantaNavigation?.NombrePlanta ?? string.Empty,
                zonaId = j.IdZona,
                zonaNombre = j.IdZonaNavigation?.NombreZona ?? string.Empty,
                depositoId = j.IdDeposito,
                depositoNombre = j.IdDepositoNavigation?.NombreDeposito ?? string.Empty,
                deptoId = j.IdDepto,
                deptoNombre = j.IdDeptoNavigation?.NombreDepto ?? string.Empty,
                esPrincipal = j.EsPrincipal == 1
            }).ToList();

            return Json(new { success = true, items = items });
        }


    }
}
