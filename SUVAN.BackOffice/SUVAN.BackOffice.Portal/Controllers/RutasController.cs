using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class RutasController : Controller
  {
    private readonly IRutasService rutasService;
    private readonly IParadasService paradasService;

    public RutasController(IRutasService rutasService, IParadasService paradasService)
    {
      this.rutasService = rutasService;
      this.paradasService = paradasService;
    }
    public async Task<IActionResult> Index()
    {
      var rutas = await rutasService.ObtenerRutas(User.GetEmpresaId());
      return View(rutas);
    }

    public async Task<IActionResult> Agregar(int id)
    {
      var agregarModel = await rutasService.GetRutaViewModel(id, User.GetEmpresaId());

      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarRutaViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await rutasService.AgregarRuta(model, User.GetEmpresaId());

        if (result)
        {
          return RedirectToAction("Index");
        }

        return View(model);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }
    }

    public async Task<IActionResult> ConfiguracionRuta()
    {
      var rutas = await rutasService.GetRutasSinConfigurar(User.GetEmpresaId());
      return View(rutas);
    }


    [HttpPost]
    public async Task<IActionResult> EliminarRuta([FromBody] AgregarRutaViewModel model)
    {
      try
      {
        await rutasService.EliminarRuta(model.RutaId, User.GetEmpresaId());


        return Ok(new { success = true });


      }
      catch (Exception ex)
      {
        return Ok(new { success = false, message = ex.Message });
      }
    }

  }
}
