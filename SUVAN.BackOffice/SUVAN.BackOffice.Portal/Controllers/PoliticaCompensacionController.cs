using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class PoliticaCompensacionController : Controller
  {
    private readonly IPoliticasCompensacionService politicasCompensacionService;

    public PoliticaCompensacionController(IPoliticasCompensacionService politicasCompensacionService)
    {
      this.politicasCompensacionService = politicasCompensacionService;
    }

    public async Task<IActionResult> Index()
    {
      var result = await politicasCompensacionService.GetPoliticasCompensacion(User.GetEmpresaId());

      return View(result);
    }

    public async Task<IActionResult> Agregar(int id)
    {
      var agregarModel = await politicasCompensacionService.GetPoliticasCompensacionesViewModel(id, User.GetEmpresaId());
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarPoliticaCompensacionViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await politicasCompensacionService.GuardarPoliticaCompensacion(model, User.GetEmpresaId());

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

    public async Task<IActionResult> Empresa()
    {
      var result = await politicasCompensacionService.GetPoliticasCompensacionEmpresa(User.GetEmpresaId());

      return View(result);
    }

    public async Task<IActionResult> AgregarEmpresa(int id)
    {
      var agregarModel = await politicasCompensacionService.GetPoliticasCompensacionesEmpresaViewModel(id, User.GetEmpresaId());
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarEmpresa(AgregarPoliticaCompensacionViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await politicasCompensacionService.GuardarPoliticaCompensacionEmpresa(model, User.GetEmpresaId());

        if (result)
        {
          return RedirectToAction("Empresa");
        }

        return View(model);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }
    }
  }
}
