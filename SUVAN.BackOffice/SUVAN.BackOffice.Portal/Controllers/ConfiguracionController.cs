using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Service.Logistica;
using SUVAN.BackOffice.Service.MensajeriaService;
using SUVAN.BackOffice.Service.Seguridad;

namespace SUVAN.BackOffice.Portal.Controllers
{

    [Authorize]
    public class ConfiguracionController : Controller
    {
        private readonly ILogger<ConfiguracionController> _logger;
        private readonly IEmpresasService empresasService;
        private readonly IConductorService conductorService;
        private readonly ITipoVehiculoService tipoVehiculoService;
        private readonly IVehiculoService vehiculoService;
        private readonly ITarifaService tarifaService;
        private readonly IConversacionesService conversacionesService;
        // Regiones
        private readonly IRegionService regionesService;
        // Plantas
        private readonly IPlantaService plantasService;
        // Departamentos
        private readonly IDeptoService deptoService;


        public ConfiguracionController(ILogger<ConfiguracionController> logger,
          IEmpresasService empresasService,
          IConductorService conductorService,
          ITipoVehiculoService tipoVehiculoService,
          IVehiculoService vehiculoService,
          ITarifaService tarifaService,
          IConversacionesService conversacionesService,
          IRegionService regionService,
          IPlantaService plantaService,
          IDeptoService deptoService)
        {
            _logger = logger;
            this.empresasService = empresasService;
            this.conductorService = conductorService;
            this.tipoVehiculoService = tipoVehiculoService;
            this.vehiculoService = vehiculoService;
            this.tarifaService = tarifaService;
            this.conversacionesService = conversacionesService;
            this.regionesService = regionService;
            this.plantasService = plantaService;
            this.deptoService = deptoService;

        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Empresas()
        {
            var empresas = await empresasService.GetEmpresas();
            return View(empresas);
        }

        public async Task<IActionResult> AgregarEmpresa(int id)
        {
            var agregarModel = await empresasService.GetEmpresasViewModel(id);
            agregarModel.TipoRegimen = empresasService.ObtenerTipoRegimen();
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarEmpresa(AgregarEmpresaViewModel model)
        {
            try
            {
                model.TipoRegimen = empresasService.ObtenerTipoRegimen();

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await empresasService.AgregarEmpresa(model);

                if (result)
                {
                    return RedirectToAction("Empresas", "Configuracion");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }


        }

        public async Task<IActionResult> Conductores()
        {
            var conductores = await conductorService.GetConductores(User.GetEmpresaId());
            return View(conductores);
        }

        public async Task<IActionResult> AgregarConductor(int id)
        {
            var agregarModel = await conductorService.GetConductorViewModel(id, User.GetEmpresaId());
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarConductor(AgregarConductorViewModel model)
        {
            model.RegimenFiscal = await conductorService.GetRegimenFiscal();
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await conductorService.AgregarConductor(model, User.GetEmpresaId());

                if (result)
                {
                    return RedirectToAction("Conductores", "Configuracion");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }


        }


        public async Task<IActionResult> TipoUnidad()
        {
            var tipoUnidades = await tipoVehiculoService.GetTipovehiculos();
            return View(tipoUnidades);
        }


        public async Task<IActionResult> AgregarTipoUnidad(int id)
        {
            var agregarModel = await tipoVehiculoService.GetTipoVehiculoViewModel(id);
            return View(agregarModel);
        }


        [HttpPost]
        public async Task<IActionResult> AgregarTipoUnidad(AgregarTipoUnidadViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await tipoVehiculoService.AgregarTipoVehiculo(model);

                if (result)
                {
                    return RedirectToAction("TipoUnidad", "Configuracion");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

        }

        public async Task<IActionResult> Unidades()
        {
            var vehiculos = await vehiculoService.GetAllVehiculos(User.GetEmpresaId());
            return View(vehiculos);
        }

        public async Task<IActionResult> AgregarUnidad(int id)
        {
            var agregarModel = await vehiculoService.GetVehiculoViewModel(id, User.GetEmpresaId());
            agregarModel.Modelos = await vehiculoService.ObtenerModelo(agregarModel.IdMarca);
            agregarModel.MarcaJson = JsonConvert.SerializeObject(agregarModel.Marcas);
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarUnidad(AgregarUnidadViewModel model)
        {
            var returnModel = await vehiculoService.GetVehiculoViewModel(0, User.GetEmpresaId());
            try
            {
                returnModel.Placas = model.Placas;
                returnModel.Vin = model.Vin;
                returnModel.TipoUnidadId = model.TipoUnidadId;
                returnModel.UnidadId = model.UnidadId;
                returnModel.Activo = model.Activo;
                returnModel.IdMarca = model.IdMarca;
                returnModel.IdModelo = model.IdModelo;

                if (!ModelState.IsValid)
                {
                    return View(returnModel);
                }

                var result = await vehiculoService.AgregarVehiculo(model, User.GetEmpresaId());

                if (result)
                {
                    var idVehiculoDetalle = await vehiculoService.AgregarDetalle(model, model.UnidadId);

                    ViewBag.MostrarConfirmacionDetalle = true;
                    ViewBag.IdVehiculoDetalle = idVehiculoDetalle;

                    return View(returnModel);
                }

                return View(returnModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(returnModel);
            }

        }

        public IActionResult Tarifas()
        {
            var model = new EstablecerTarifaViewModel();
            model.Rutas = tarifaService.ObtenerRutas(User.GetEmpresaId());
            model.TipoTarifas = tarifaService.ObtenerTipoTarifa();

            return View(model);
        }

        [HttpPost]
        public IActionResult ObtenerPrecioTarifa(EmpresaTarifaModel model)
        {
            var paraRutaModel = tarifaService.ObtenerPrecioTarifa(model);
            return Json(paraRutaModel);
        }
        [HttpPost]
        public async Task<IActionResult> ActualizaPrecioTarifa(EmpresaTarifaModel model)
        {
            var paraRutaModel = await tarifaService.ActualizaPrecioTarifa(model);
            return Json(paraRutaModel);
        }

        public IActionResult ChatBox()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerInformacionChat(MensajeConversacion model)
        {
            model.UsuarioIdCreacion = User.GetUserId();
            model.EmpresaId = User.GetEmpresaId();

            var resultConversacion = await conversacionesService.ObtenerConversacion(model);
            return Json(resultConversacion.Data);
        }
        [HttpPut]
        public async Task<IActionResult> CerrarConversacion([FromQuery] int conversacionId, int estatus)
        {
            var resultConversacion = await conversacionesService.ModificarEstatus(conversacionId, estatus);
            if (resultConversacion.Data == 1)
                return Ok(resultConversacion);
            else
                return BadRequest();
        }


        public async Task<IActionResult> ReporteOperadores()
        {
            var reporte = await conductorService.ReporteOperadores();

            return View(reporte);
        }


        // Region
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

                var result = await regionesService.AgregarRegion(model);

                if (result)
                {
                    return RedirectToAction("regiones", "Configuracion");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }


        }

        // Plantas
        public async Task<IActionResult> Plantas()
        {
            var plantas = await plantasService.GetPlantas(User.GetEmpresaId());
            return View(plantas);
        }

        public async Task<IActionResult> AgregarPlanta(int idRegion = 0, int idPlanta = 0)
        {
            var agregarModel = await plantasService.GetPlantaViewModel(User.GetEmpresaId(), idRegion, idPlanta);
            ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPlanta(PlantaViewModel model)
        {
            try
            {
                model.id_emp = (short)User.GetEmpresaId();
                if (!ModelState.IsValid)
                {
                    ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
                    return View(model);
                }

                var result = await plantasService.AgregarPlanta(model);

                if (result)
                {
                    return RedirectToAction("Plantas", "Configuracion");
                }

                ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
                return View(model);
            }
        }
        // Deptos
        public async Task<IActionResult> Depto()
        {
            var deptos = await deptoService.GetDeptos(User.GetEmpresaId());
            return View("Depto", deptos);
        }

        public async Task<IActionResult> AgregarDepto(short idRegion = 0, short idPlanta = 0, short idZona = 0, short idDeposi = 0, short idDepto = 0)
        {
            var agregarModel = await deptoService.GetDeptoViewModel(User.GetEmpresaId(), idRegion, idPlanta, idZona, idDeposi, idDepto);
            ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
            ViewBag.Plantas = await plantasService.GetPlantas(User.GetEmpresaId());
            ViewBag.Zonas = await deptoService.GetZonas(User.GetEmpresaId());
            ViewBag.Depositos = await deptoService.GetDepositos(User.GetEmpresaId());
            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDepto(DeptoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
                    ViewBag.Plantas = await plantasService.GetPlantas(User.GetEmpresaId());
                    ViewBag.Zonas = await deptoService.GetZonas(User.GetEmpresaId());
                    ViewBag.Depositos = await deptoService.GetDepositos(User.GetEmpresaId());
                    return View(model);
                }

                var result = await deptoService.AgregarDepto(model, User.GetEmpresaId());

                if (result)
                {
                    return RedirectToAction("Depto", "Configuracion");
                }

                ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
                ViewBag.Plantas = await plantasService.GetPlantas(User.GetEmpresaId());
                ViewBag.Zonas = await deptoService.GetZonas(User.GetEmpresaId());
                ViewBag.Depositos = await deptoService.GetDepositos(User.GetEmpresaId());
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Regiones = await regionesService.GetRegiones(User.GetEmpresaId());
                ViewBag.Plantas = await plantasService.GetPlantas(User.GetEmpresaId());
                ViewBag.Zonas = await deptoService.GetZonas(User.GetEmpresaId());
                ViewBag.Depositos = await deptoService.GetDepositos(User.GetEmpresaId());
                return View(model);
            }
        }


    }
}
