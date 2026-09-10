using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;

namespace SUVAN.BackOffice.Portal.Controllers
{
    public class TallerController : Controller
    {
        private readonly ILogger<TallerController> _logger;
        private readonly ITallerService taller;

        public TallerController(ILogger<TallerController> logger,
        ITallerService talleres)

        {
            _logger = logger;
            this.taller = talleres;

        }

        public async Task<IActionResult> Index()
        {
            var t = await taller.GetTaller(User.GetEmpresaId());
            return View(t);
        }

        public async Task<IActionResult> AgregarTaller(int id)
        {
            var idEmpresa = User.GetEmpresaId();
            var model = await taller.GetTallerViewModel(id, idEmpresa);
            
            //caraga de regiones iniciales para el primer selector  (regiones)
            model.Regiones = await taller.GetRegions(idEmpresa);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarTaller(TallerViewModel model)
        {
            try
            {
                var idEmpresa = User.GetEmpresaId();

                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .FirstOrDefault()?.ErrorMessage ?? "Hay errores en el formulario.";
                    return Json(new { success = false, message = errorMessage });

                    //model.Regiones = await taller.GetRegions(idEmpresa);
                    //return View(model);
                }

                var result = await taller.AgregarTaller(model, idEmpresa);

                if (result)
                {
                    return Json(new { success = true, message = "Taller guardado correctamente" });

                }

                return Json(new { success = false, message = "No se pudo guardar el taller" });

                //model.Regiones = await taller.GetRegions(User.GetEmpresaId());
                //return View(model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
                //ModelState.AddModelError(string.Empty, ex.Message);
                //model.Regiones = await taller.GetRegions(User.GetEmpresaId());
                //return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarTaller([FromBody] TallerViewModel model)
        {
            try
            {
                await taller.EliminarTaller(model.IdTaller);

                return Json(new { success = true, message = "Taller eliminado correctamente." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetPlantas(int regionId)
        {
            var idEmpresa = User.GetEmpresaId();
            var plantas = await taller.GetPlantasByRegion(idEmpresa, regionId);
            return Json(plantas);
        }

        [HttpGet]
        public async Task<JsonResult> GetZonas(int plantaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var zonas = await taller.GetZonasByPlanta(idEmpresa, plantaId);
            return Json(zonas);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepositos(int zonaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var depositos = await taller.GetDepositosByZona(idEmpresa, zonaId);
            return Json(depositos);
        }
    }
}
