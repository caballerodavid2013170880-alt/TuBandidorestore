using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Administrativo;
using SUVAN.BackOffice.Models.ViewModel.Administrativo;
using System.Security.Claims;

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

        /// <summary>
        /// Obtiene variables de contexto de sesión del usuario para validación de datos.
        /// </summary>
        private (int idEmpresa, int idUsuario, int? idRegion, int? idPlanta, int? idZona, int? idDeposito) GetUserContext()
        {
            int idEmpresa = User.GetEmpresaId();
            int idUsuario = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Reemplazar estas asignaciones nulas cuando los claims de jerarquía estén implementados
            int? idRegion = null;
            int? idPlanta = null;
            int? idZona = null;
            int? idDeposito = null;

            return (idEmpresa, idUsuario, idRegion, idPlanta, idZona, idDeposito);
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        // =========== Region ==============
        public async Task<IActionResult> Regiones()
        {
            var usr = GetUserContext();
            var regiones = await regionesService.GetRegiones(usr.idEmpresa);
            return View(regiones);
        }

        public async Task<IActionResult> AgregarRegion(int id)
        {
            var usr = GetUserContext();
            var agregarModel = await regionesService.GetRegionViewModel(usr.idEmpresa, id);
            return View(agregarModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarRegion(RegionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos o incompletos." });

                var usr = GetUserContext();
                var result = await regionesService.AgregarRegion(model, usr.idEmpresa);

                if (result)
                    return Json(new { success = true, message = model.IdRegion == 0 ? "Región registrada correctamente." : "Región actualizada correctamente." });

                return Json(new { success = false, message = "No se pudo guardar la región." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar región");
                return Json(new { success = false, message = ex.Message });
            }
        }
        // ============== Region FIN ==============

        // ============== Plantas ==============
        [HttpGet]
        public async Task<IActionResult> Plantas()
        {
            var usr = GetUserContext();
            var plantas = await plantaService.GetPlantas(usr.idEmpresa);
            return View(plantas);
        }

        public async Task<IActionResult> AgregarPlanta(int id)
        {
            var usr = GetUserContext();
            var model = await plantaService.GetPlantaViewModel(usr.idEmpresa, id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPlanta(PlantaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos o incompletos." });

                var usr = GetUserContext();
                var result = await plantaService.AgregarPlanta(model, usr.idEmpresa);

                if (result)
                    return Json(new { success = true, message = model.IdPlanta == 0 ? "Planta registrada correctamente." : "Planta actualizada correctamente." });

                return Json(new { success = false, message = "No se pudo guardar la planta." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar planta");
                return Json(new { success = false, message = ex.Message });
            }
        }
        // ============== Planta FIN ==============

        // ============== Zonas ==============
        [HttpGet]
        public async Task<IActionResult> Zonas()
        {
            var usr = GetUserContext();
            var zona = await zonaService.GetZona(usr.idEmpresa);
            return View(zona);
        }

        public async Task<IActionResult> AgregarZona(int id)
        {
            var usr = GetUserContext();
            var agregarModel = await zonaService.GetZonaViewModel(id, usr.idEmpresa);
            return View(agregarModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarZona(ZonaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos o incompletos." });

                var usr = GetUserContext();
                var result = await zonaService.AgregarZona(model, usr.idEmpresa);

                if (result)
                    return Json(new { success = true, message = model.IdZona == 0 ? "Zona registrada correctamente." : "Zona actualizada correctamente." });

                return Json(new { success = false, message = "No se pudo guardar la zona." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar zona");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarZona(ZonaViewModel model)
        {
            try
            {
                var usr = GetUserContext();
                await zonaService.EliminarZona(model.IdZona, usr.idEmpresa);
                return Json(new { success = true, message = "Zona eliminada." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ============== Zona FIN ==============

        // ============== Depósitos ==============
        [HttpGet]
        public async Task<IActionResult> Depositos()
        {
            var usr = GetUserContext();
            var depositos = await depositosService.GetDepositos(usr.idEmpresa);
            return View(depositos);
        }

        public async Task<IActionResult> AgregarDeposito(int id)
        {
            var usr = GetUserContext();
            var model = await depositosService.GetDepositoViewModel(usr.idEmpresa, id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarDeposito(DepositoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Datos inválidos o incompletos." });

                var usr = GetUserContext();
                var result = await depositosService.AgregarDeposito(model, usr.idEmpresa);

                if (result)
                    return Json(new { success = true, message = model.IdDeposito == 0 ? "Depósito registrado correctamente." : "Depósito actualizado correctamente." });

                return Json(new { success = false, message = "No se pudo guardar el depósito." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar deposito");
                return Json(new { success = false, message = ex.Message });
            }
        }
        /*
        [HttpGet]
        public async Task<JsonResult> ObtenerZonas(int plantaId)
        {
            var usr = GetUserContext();
            var zonas = await depositosService.GetZonasByPlanta(usr.idEmpresa, plantaId);
            return Json(zonas);
        }
        */  //Descartado por alineacion de catalogos jerarquia 2007
        // ============== Departamentos ==============
        [HttpGet]
        public async Task<IActionResult> Departamentos()
        {
            var usr = GetUserContext();
            var deptos = await deptoService.GetDepto(usr.idEmpresa);
            return View(deptos);
        }

        public async Task<IActionResult> AgregarDepto(int id)
        {
            var usr = GetUserContext();
            var model = await deptoService.GetDeptoViewModel(usr.idEmpresa, id);
            return View(model);
        }

        /// <summary>
        /// Procesa la persistencia (alta o actualización) del registro de departamento.
        /// Retorna a la vista principal con un TempData en caso de éxito.
        /// </summary>
        /// <param name="model">Modelo de datos capturado en la vista de Departamento.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarDepto(DeptoViewModel model)
        {
            try
            {
                var usr = GetUserContext();

                if (!ModelState.IsValid)
                {
                    // Si el modelo es inválido, recargar las listas para volver a mostrar el formulario
                    var recargar = await deptoService.GetDeptoViewModel(usr.idEmpresa, model.IdDepto);
                    model.Regiones = recargar.Regiones;
                    model.Plantas = recargar.Plantas;
                    model.Zonas = recargar.Zonas;
                    model.Depositos = recargar.Depositos;
                    return View(model);
                }

                var result = await deptoService.AgregarDepto(model, usr.idEmpresa);

                if (result)
                {
                    // Almacenar el mensaje de éxito para que table.js lo lea y dispare el SweetAlert
                    TempData["Mensaje"] = model.IdDepto == 0
                        ? "Departamento registrado correctamente."
                        : "Departamento actualizado correctamente.";

                    return RedirectToAction("Departamentos", "Administrativo");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar depto");
                ModelState.AddModelError(string.Empty, ex.Message);

                var usr = GetUserContext();
                var recargar = await deptoService.GetDeptoViewModel(usr.idEmpresa, model.IdDepto);
                model.Regiones = recargar.Regiones;
                model.Plantas = recargar.Plantas;
                model.Zonas = recargar.Zonas;
                model.Depositos = recargar.Depositos;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlantasPorRegion(int idRegion)
        {
            var usr = GetUserContext();
            var plantas = await deptoService.GetPlantasPorRegion(usr.idEmpresa, idRegion);
            return Json(plantas);
        }

        [HttpGet]
        public async Task<IActionResult> GetZonasPorPlanta(int idRegion, int idPlanta)
        {
            var usr = GetUserContext();
            var zonas = await deptoService.GetZonasPorPlanta(usr.idEmpresa, idRegion, idPlanta);
            return Json(zonas);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepositosPorZona(int idRegion, int idPlanta, int idZona)
        {
            var usr = GetUserContext();
            var depositos = await deptoService.GetDepositosPorZona(usr.idEmpresa, idRegion, idPlanta, idZona);
            return Json(depositos);
        }
    }
}
