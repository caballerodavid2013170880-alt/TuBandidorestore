using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class PerfilesController : Controller
  {
    private readonly ILogger<PerfilesController> logger;
    private readonly IPerfilService perfilService;

    public PerfilesController(ILogger<PerfilesController> logger, IPerfilService perfilService)
    {
      this.logger = logger;
      this.perfilService = perfilService;
    }

    public async Task<IActionResult> Index()
    {
      var perfiles = await perfilService.GetPerfiles();
      return View(perfiles);
    }

    public async Task<IActionResult> Agregar(int id)
    {
      var agregarModel = await perfilService.GetPerfilViewModel(id);
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarPerfilViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var perfilId = await perfilService.AgregarPerfil(model);

      if (perfilId > 0)
      {
        return RedirectToAction("Index", "Perfiles");
      }

      return View(model);

    }

    [HttpPost]
    public async Task<IActionResult> Eliminar([FromBody] DeletePerfilViewModel model)
    {
      try
      {
        var result = await perfilService.EliminarPerfil(model.PerfilId);

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
