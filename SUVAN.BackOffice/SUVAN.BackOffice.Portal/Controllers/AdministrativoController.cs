using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
using SUVAN.BackOffice.Service.Logistica;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    public class AdministrativoController : Controller
    {
        private readonly ILogger<AdministrativoController> _logger;
        private readonly IRegionService regionesService;
        private readonly IPlantaService plantaService;
        private readonly IZonaService zonaService;
        private readonly IDepositoService depositosService;
        private readonly IDeptoService deptoService;

        public AdministrativoController(
            ILogger<AdministrativoController> logger,
            IRegionService regionService,
            IPlantaService plantaService,
            IZonaService zonaService,
            IDepositoService depositosService,
            IDeptoService deptoService)
        {
            _logger = logger;
            this.regionesService = regionService;
            this.plantaService = plantaService;
            this.zonaService = zonaService;
            this.depositosService = depositosService;
            this.deptoService = deptoService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }


        // =========== Region ==============
        //[HttpGet]
        //[Route("/Regiones")]
        public async Task<IActionResult> Regiones()
        {
            var regiones = await regionesService.GetRegiones(User.GetEmpresaId());
            return View(regiones);
        }

        public async Task<IActionResult> AgregarRegion(int id)
        {
            var agregarModel = await regionesService.GetRegionViewModel(User.GetEmpresaId(), id);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarRegion(RegionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                int idEmpresa = User.GetEmpresaId();
                var result = await regionesService.AgregarRegion(model, idEmpresa);

                if (result)
                {
                    TempData["Mensaje"] = model.IdRegion == 0
                        ? "Región registrada correctamente."
                        : "Región actualizada correctamente.";
                    return RedirectToAction("Regiones", "Administrativo");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
        // ============== Region FIN ==============

        // ============== Plantas ==============
        [HttpGet]
        //[Route("/Plantas")]
        public async Task<IActionResult> Plantas()
        {
            var plantas = await plantaService.GetPlantas(User.GetEmpresaId());
            return View(plantas);
        }

        public async Task<IActionResult> AgregarPlanta(int id)
        {
            var model = await plantaService.GetPlantaViewModel(User.GetEmpresaId(), id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPlanta(PlantaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var recargar = await plantaService.GetPlantaViewModel(User.GetEmpresaId(), model.IdPlanta);
                    model.Regiones = recargar.Regiones;
                    return View(model);
                }

                var result = await plantaService.AgregarPlanta(model, User.GetEmpresaId());

                if (result)
                {
                    TempData["Mensaje"] = model.IdPlanta == 0
                        ? "Planta registrada correctamente."
                        : "Planta actualizada correctamente.";
                    return RedirectToAction("Plantas", "Administrativo");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var recargar = await plantaService.GetPlantaViewModel(User.GetEmpresaId(), model.IdPlanta);
                model.Regiones = recargar.Regiones;
                return View(model);
            }
        }
        // ============== Planta FIN ==============

        // ============== Zonas ==============
        [HttpGet]
        //[Route("/Zonas")]
        public async Task<IActionResult> Zonas()
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
                    var recargar = await zonaService.GetZonaViewModel(model.ZonaId, IdEmpresa);
                    model.ListadoRegiones = recargar.ListadoRegiones;
                    model.ListadoPlantas = recargar.ListadoPlantas;
                    return View(model);
                }

                var result = await zonaService.AgregarZona(model, IdEmpresa);

                if (result)
                {
                    TempData["Mensaje"] = model.ZonaId == 0
                        ? "Registro insertado correctamente."
                        : "Registro actualizado correctamente.";

                    return RedirectToAction("Zonas", "Administrativo");
                }

                var reload = await zonaService.GetZonaViewModel(model.ZonaId, IdEmpresa);
                model.ListadoRegiones = reload.ListadoRegiones;
                model.ListadoPlantas = reload.ListadoPlantas;

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

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
            var plantas = await depositosService.GetPlantasByRegion(idEmpresa, regionId);
            return Json(plantas);
        }
        // ============== Zona FIN ==============

        // ============== Depósitos ==============
        [HttpGet]
        //[Route("/Depositos")]
        public async Task<IActionResult> Depositos()
        {
            var depositos = await depositosService.GetDepositos(User.GetEmpresaId());
            return View(depositos);
        }

        public async Task<IActionResult> AgregarDeposito(int id)
        {
            var agregarModel = await depositosService.GetDepositoViewModel(User.GetEmpresaId(), id);

            agregarModel.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());
            agregarModel.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());
            agregarModel.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());

            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDeposito(DepositoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());
                    model.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());
                    model.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());
                    return View(model);
                }

                var result = await depositosService.AgregarDeposito(model);

                if (result)
                {
                    return RedirectToAction("Depositos", "Administrativo");
                }

                model.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());
                model.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());
                model.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());
                model.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());
                model.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());

                return View(model);
            }
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerZonas(int plantaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var zonas = await depositosService.GetZonasByPlanta(idEmpresa, plantaId);
            return Json(zonas);
        }

        // ============== Departamentos ==============
        //[Route("/Departamentos")]
        [HttpGet]
        public async Task<IActionResult> Departamentos()
        {
            var deptos = await deptoService.GetDepto(User.GetEmpresaId());
            return View(deptos);
        }

        public async Task<IActionResult> AgregarDepto(int id)
        {
            var model = await deptoService.GetDeptoViewModel(User.GetEmpresaId(), id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDepto(DeptoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var recargar = await deptoService.GetDeptoViewModel(User.GetEmpresaId(), model.IdDepto);
                    model.Regiones = recargar.Regiones;
                    model.Plantas = recargar.Plantas;
                    model.Zonas = recargar.Zonas;
                    model.Depositos = recargar.Depositos;
                    return View(model);
                }

                var result = await deptoService.AgregarDepto(model, User.GetEmpresaId());

                if (result)
                {
                    TempData["Mensaje"] = model.IdDepto == 0
                        ? "Departamento registrado correctamente."
                        : "Departamento actualizado correctamente.";
                    return RedirectToAction("Departamentos", "Administrativo");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var recargar = await deptoService.GetDeptoViewModel(User.GetEmpresaId(), model.IdDepto);
                model.Regiones = recargar.Regiones;
                model.Plantas = recargar.Plantas;
                model.Zonas = recargar.Zonas;
                model.Depositos = recargar.Depositos;
                model.CascadeJson = recargar.CascadeJson;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlantasPorRegion(int idRegion)
        {
            var plantas = await deptoService.GetPlantasPorRegion(User.GetEmpresaId(), idRegion);
            return Json(plantas);
        }

        [HttpGet]
        public async Task<IActionResult> GetZonasPorPlanta(int idRegion, int idPlanta)
        {
            var zonas = await deptoService.GetZonasPorPlanta(User.GetEmpresaId(), idRegion, idPlanta);
            return Json(zonas);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepositosPorZona(int idRegion, int idPlanta, int idZona)
        {
            var depositos = await deptoService.GetDepositosPorZona(User.GetEmpresaId(), idRegion, idPlanta, idZona);
            return Json(depositos);
        }
    }
}
