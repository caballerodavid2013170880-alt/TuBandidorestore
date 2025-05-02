using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Ingresos;

namespace SUVAN.BackOffice.Portal.Controllers.Ingresos
{

  [Authorize]
  public class ReporteIngresosController : Controller
  {
    private readonly IReporteIngresosService reporteIngresosService;

    public ReporteIngresosController(IReporteIngresosService reporteIngresosService)
    {
      this.reporteIngresosService = reporteIngresosService;
    }
    public async Task<IActionResult> Index()
    {
      var model = await reporteIngresosService.InitReporteIngresosGlobal();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ReporteIngresosViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await reporteIngresosService.ReporteIngresosSearch(model);
        //result.Desde = null;
        //result.Hasta = null;
        return View(result);

      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }
    }

    public async Task<IActionResult> Empresa()
    {
      var model = await reporteIngresosService.InitReporteIngresosGlobal();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Empresa(ReporteIngresosViewModel model)
    {
      try
      {
        model.EmpresaId = User.GetEmpresaId();
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await reporteIngresosService.ReporteIngresosEmpresaSearch(model);
        //result.Desde = null;
        //result.Hasta = null;
        return View(result);

      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }
    }
  }
}
