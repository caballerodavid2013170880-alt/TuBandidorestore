using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class VariablesController : Controller
  {
    private readonly IVariablesService variablesService;

    public VariablesController(IVariablesService variablesService)
    {
      this.variablesService = variablesService;
    }

    public async Task<IActionResult> Index()
    {
      var variables = await variablesService.GetVariablesEmpresa(User.GetEmpresaId());
      return View(variables);
    }

    public async Task<IActionResult> Globales()
    {
      var variables = await variablesService.GetVariablesGlobales();
      return View(variables);
    }

    [HttpPost]
    public async Task<IActionResult> Index(AgregarVariableViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);

        }
        var result = await variablesService.AgregarVariables(model, User.GetEmpresaId(), EnumTipoVariable.Empresa, User.Identity!.Name!);
        if (!result)
        {
          return View(model);
        }
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {

        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }


    }

    [HttpPost]
    public async Task<IActionResult> Globales(AgregarVariableViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }
        var result = await variablesService.AgregarVariables(model, User.GetEmpresaId(), EnumTipoVariable.Global, User.Identity!.Name!);
        if (!result)
        {
          return View(model);
        }
        return RedirectToAction("Globales");
      }
      catch (Exception ex)
      {

        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }


    }
  }
}
