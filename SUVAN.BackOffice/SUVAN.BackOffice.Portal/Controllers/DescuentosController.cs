using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Service.Configuracion;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class DescuentosController : Controller
  {
    private readonly ICodigoDescuentoService codigoDescuentoService;

    public DescuentosController(ICodigoDescuentoService codigoDescuentoService)
    {
      this.codigoDescuentoService = codigoDescuentoService;
    }
    public async Task<IActionResult> Index()
    {
      var codigoDescuentos = await codigoDescuentoService.GetCodigoDescuentos();

      return View(codigoDescuentos);
    }

    public async Task<IActionResult> Agregar(int id)
    {
      var agregarModel = await codigoDescuentoService.GetCodigoDescuentoViewModel(id);
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarDescuentoViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await codigoDescuentoService.GuardarCodigoDescuento(model);

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
  }
}
