using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Seguridad;
using System.Reflection.Metadata;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class EstacionesController : Controller
  {
    private readonly IParadasService paradasService;

    public EstacionesController(IParadasService paradasService)
    {
      this.paradasService = paradasService;
    }
    public async Task<IActionResult> Index()
    {
      var paradas = await paradasService.GetParadas();
      return View(paradas);
    }

    public async Task<IActionResult> AgregarEstacion(int id)
    {
      var agregarModel = await paradasService.GetEstacionViewModel(id);
      return View(agregarModel);
    }


    [HttpPost]
    public async Task<IActionResult> AgregarEstacion(AgregarEstacionViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await paradasService.AgregarEstacion(model);

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
    //[FromBody] BuscarViewModel model

    [HttpGet]
    public async Task<IActionResult> BuscarEstacion(string query)
    {
      try
      {
        var respuesta = await paradasService.GetParadasByName(query);

        return Ok(new { success = true, result = respuesta });
      }
      catch (Exception ex)
      {
        return BadRequest(new { success = false, message = ex.Message });
      }
    }
  }
}
