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

    public UsuariosController(ILogger<PerfilesController> logger, IAdminService adminService)
    {
      this.logger = logger;
      this.adminService = adminService;
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
  }
}
