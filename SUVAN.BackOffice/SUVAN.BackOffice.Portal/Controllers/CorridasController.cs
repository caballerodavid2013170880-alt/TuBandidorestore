using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Contenidos;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class CorridasController : Controller
  {
    private readonly ICorridasService corridasService;

    public CorridasController(ICorridasService corridasService)
    {
      this.corridasService = corridasService;
    }

    public async Task<IActionResult> Index()
    {
      var corridas = await corridasService.GetCorridas(User.GetEmpresaId());
      return View(corridas);
    }

    public async Task<IActionResult> Agregar(int id)
    {
      AgregarCorridaViewModel viewModel = new();
      try
      {
        viewModel = await corridasService.GetCorridasViewModel(id, User.GetEmpresaId());
        return View(viewModel);
      }
      catch (Exception ex)
      {

        ModelState.AddModelError(string.Empty, ex.Message);
        return View(viewModel);
      }

    }

    [HttpPost]
    public async Task<IActionResult> Agregar(AgregarCorridaViewModel model, string agregarCorrida)
    {
      try
      {

        if (!string.IsNullOrEmpty(agregarCorrida) && agregarCorrida == "+")
        {
          // Agregar una nueva corrida

          model.Corridas.Add(await corridasService.AddNuevaCorrida());
          ModelState.Clear();
          return View(model);
        }
        else if (!string.IsNullOrEmpty(agregarCorrida) && agregarCorrida == "-")
        {
          // Eliminar una corrida
          model.Corridas.RemoveAt(model.Corridas.Count - 1);
          ModelState.Clear();
          return View(model);
        }
        else if (!string.IsNullOrEmpty(agregarCorrida) && agregarCorrida == "Guardar")
        {

          if (!ModelState.IsValid)
          {
            return View(model);
          }

          var result = await corridasService.AgregarCorrida(model, User.GetEmpresaId());

          if (result)
          {
            return RedirectToAction("Index");
          }

          return View(model);

        }
        else
        {
          // elimina una corrida que ya fue guardada
          var corridaId = int.Parse(agregarCorrida);
          await corridasService.EliminarHorarioCorrida(corridaId, User.GetEmpresaId());
          var itemCorrida = model.Corridas.FirstOrDefault(x => x.CorridaId == corridaId);
          model.Corridas.Remove(itemCorrida!);
          return View(model);
        }


      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, ex.Message);
        return View(model);
      }
    }

    public async Task<IActionResult> DetalleRuta(int id)
    {
      var viewModel = await corridasService.GetDetalleRutaViewModel(id, User.GetEmpresaId());
      return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> DetalleRuta(DetalleRutaViewModel model)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return View(model);
        }

        var result = await corridasService.AgregarDetalleRuta(model, User.GetEmpresaId());

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

    [HttpPost]
    public async Task<IActionResult> EliminarCorrida([FromBody] DeleteCorridaViewModel model)
    {
      try
      {
        var result = await corridasService.EliminaCorrida(model.RutaId, User.GetEmpresaId());

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

    [HttpPost]
    public async Task<IActionResult> EliminarHorarioCorrida([FromBody] DeleteCorridaViewModel model)
    {
      try
      {
        var result = await corridasService.EliminarHorarioCorrida(model.RutaId, User.GetEmpresaId());


        return Ok(new { success = result });


      }
      catch (Exception ex)
      {
        return Ok(new { success = false, message = ex.Message });
      }
    }
  }
}
