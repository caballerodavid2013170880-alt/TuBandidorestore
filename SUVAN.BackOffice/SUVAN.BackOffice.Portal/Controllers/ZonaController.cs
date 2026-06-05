using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class ZonaController : Controller
    {
        private readonly ILogger<ZonaController> _logger;
        private readonly IZonaService zonaService;

        public ZonaController(ILogger<ZonaController> logger,
        IZonaService zonaService)

        {
            _logger = logger;
            this.zonaService = zonaService;

        }
        public async Task<IActionResult> Index()
        {
            var zona = await zonaService.GetZona(User.GetEmpresaId());
            return View(zona);
        }

        public async Task<IActionResult> AgregarZona(int id)
        {
            var agregarModel = await zonaService.GetZonaViewModel(id, User.GetEmpresaId());
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarZona(ZonaViewModel model)
        {
            try
            {
                int IdEmpresa = User.GetEmpresaId();

                if (!ModelState.IsValid)
                {
                    //si el modelo es invalido recarga catalogos para que no salgan vacios
                    var recargar = await zonaService.GetZonaViewModel(model.ZonaId, IdEmpresa);
                    model.ListadoRegiones = recargar.ListadoRegiones;
                    model.ListadoPlantas = recargar.ListadoPlantas;
                    return View(model);
                }
            //}
                var result = await zonaService.AgregarZona(model, IdEmpresa);

                if (result)
                {
                    TempData["Mensaje"] = model.ZonaId == 0
                        ? "Registro insertado correctamente."
                        : "Registro actualizado correctamente.";

                    return RedirectToAction("Index", "Zona");
                }

                var reload = await zonaService.GetZonaViewModel(model.ZonaId, IdEmpresa);
                model.ListadoRegiones = reload.ListadoRegiones;
                model.ListadoPlantas = reload.ListadoPlantas;

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                //recrgar en caso de excepcion 
                var recargar = await zonaService.GetZonaViewModel(model.ZonaId, User.GetEmpresaId());
                model.ListadoRegiones = recargar.ListadoRegiones;
                model.ListadoPlantas = recargar.ListadoPlantas;

                return View(model);
            }


        }

        [HttpPost]
        public async Task<IActionResult> EliminarZona([FromBody] ZonaViewModel model)
        {
            try
            {
                await zonaService.EliminarZona(model.ZonaId);


                return Ok(new { success = true });


            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<JsonResult> ObtenerPlantas(int regionId)
        {
            var idEmpresa = User.GetEmpresaId();
            var plantas = await zonaService.ObtenerPlantasPorRegion(idEmpresa, regionId);
            return Json(plantas);
        }
    }
}
