using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Ingresos;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Ingresos;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class ChoferUnidadController : Controller
  {
    private readonly IChoferUnidadService choferUnidadService;

    public ChoferUnidadController(IChoferUnidadService choferUnidadService)
    {
      this.choferUnidadService = choferUnidadService;
    }
    public async Task<IActionResult> Index()
    {
      var choferUnidadViewModel = await choferUnidadService.GetChoferUnidadViewModel(User.GetEmpresaId());
      return View(choferUnidadViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ChoferUnidadViewModel model)
    {
      if (!ModelState.IsValid)
      {
        model = await choferUnidadService.GetChoferUnidadViewModel(User.GetEmpresaId());
        model.RutaId = model.RutaId;
        model.IdHoSeleccionado = model.HorarioId;
        return View(model);
      }

      model = await choferUnidadService.GetReporte(model, User.GetEmpresaId());

      return View(model);
    }

    public async Task<IActionResult> Agregar()
    {
      var choferUnidadViewModel = await choferUnidadService.GetChoferUnidadViewModel(User.GetEmpresaId());
      return View(choferUnidadViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] ChoferUnidadAgregarViewModel model)
    {
      try
      {
        var result = await choferUnidadService.AgregarChoferUnidad(model);

        if (result)
        {
          return Ok(new { success = true });
        }

        return Ok(new { success = false });

      }
      catch (Exception ex)
      {
        return Ok(new { success = false, message = ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> ConsultarChoferesUnidad([FromBody] ChoferUnidadAgregarViewModel model)
    {
      try
      {
        var result = await choferUnidadService.ConsultarChoferesUnidad(model, User.GetEmpresaId());

        if (result != null)
        {
          return Ok(new { success = true, data = result });
        }

        return Ok(new { success = false });

      }
      catch (Exception ex)
      {
        return Ok(new { success = false, message = ex.Message });
      }
    }

    public async Task<IActionResult> Reasignar()
    {
      var reasignar = await choferUnidadService.GetReasignarChoferUnidad(User.GetEmpresaId());
      return View(reasignar);


    }

    [HttpPost]
    public async Task<IActionResult> Reasignar(ConsultaReasingacionViewModel model)
    {
      var reasignar = await choferUnidadService.BuscarAsignaciones(model, User.GetEmpresaId());
      return View(reasignar);
    }

    [HttpPost]
    public async Task<IActionResult> ReasignarChoferUnidad([FromBody] ReasignarChoferViewModel model)
    {
      try
      {
        await choferUnidadService.GuardarReasignacion(model, User.GetEmpresaId());


        return Ok(new { success = true });


      }
      catch (Exception ex)
      {
        return Ok(new { success = false, message = ex.Message });
      }
    }

    [HttpPost]
    public async Task<IActionResult> EliminarAsignacionChoferUnidad([FromBody] ReasignarChoferViewModel model)
    {
      try
      {
        await choferUnidadService.EliminarAsignacionChoferUnidad(model, User.GetEmpresaId());


        return Ok(new { success = true });


      }
      catch (Exception ex)
      {
        return Ok(new { success = false, message = ex.Message });
      }
    }

  }
}
