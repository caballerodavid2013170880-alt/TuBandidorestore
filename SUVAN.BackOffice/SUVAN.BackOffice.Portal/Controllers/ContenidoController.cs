using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class ContenidoController : Controller
  {
    private readonly ILogger<ContenidoController> _logger;
    private readonly IContenidoService contenidoService;

    public ContenidoController(ILogger<ContenidoController> logger, IContenidoService contenidoService)
    {
      _logger = logger;
      this.contenidoService = contenidoService;
    }
    public async Task<IActionResult> Index()
    {
      var contenidos = await contenidoService.GetAllGeneral();

      return View(contenidos);
    }

    public async Task<IActionResult> PreguntasFrecuentes()
    {
      var contenidos = await contenidoService.GetAllPreguntas();

      return View(contenidos);
    }

    public async Task<IActionResult> AgregarPregunta(int id)
    {
      var agregarModel = await contenidoService.GetContenidoViewModel(id);
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarPregunta(AgregarContenidoViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var result = await contenidoService.AgregarContenidoGeneral(model, EnumTipoContenido.PreguntasFrecuentes);

      if (result)
      {
        return RedirectToAction("PreguntasFrecuentes", "Contenido");
      }

      return View(model);

    }

    [HttpPost]
    public async Task<IActionResult> ActualizarOrden([FromBody] OrdenarContenidoViewModel model)
    {
      try
      {
        bool contenidos = await contenidoService.ActualizarOrden(model.NuevoOrden, EnumTipoContenido.PreguntasFrecuentes);

        return Ok(new { success = true, message = "Orden actualizado correctamente." });
      }
      catch (Exception ex)
      {
        return BadRequest(new { success = false, message = ex.Message });
      }
    }

    public async Task<IActionResult> Agregar(int id)
    {
      var agregarModel = await contenidoService.GetContenidoViewModel(id);
      return View(agregarModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarContenidoViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      var result = await contenidoService.AgregarContenidoGeneral(model, EnumTipoContenido.General);

      if (result)
      {
        return RedirectToAction("Index", "Contenido");
      }

      return View(model);

    }

    [HttpPost]
    public async Task<IActionResult> EliminarGeneral([FromBody] DeleteContenidoViewModel model)
    {
      try
      {
        var result = await contenidoService.EliminarContenido(model.ContenidoId);

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
