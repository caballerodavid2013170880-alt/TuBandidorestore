using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Models.ViewModel.Enums;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Contenidos;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Service.Seguridad;
using SUVAN.BackOffice.Models.Configuracion.Tarifas;
using SUVAN.BackOffice.Portal.Helper;
using Microsoft.AspNetCore.Authorization;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Service.MensajeriaService;
using SUVAN.BackOffice.Models.Facturacion;
using SUVAN.BackOffice.Service.Logistica;
using Newtonsoft.Json;
using SUVAN.BackOffice.Database.Entities;

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
        private readonly IPlantaService plantaService;
        // Depositos
        private readonly IDepositoService depositosService; //SE DECLARA el servicio de depósitos para poder usarlo en los métodos

        public ConfiguracionController(ILogger<ConfiguracionController> logger,
        IEmpresasService empresasService,
        IConductorService conductorService,
        ITipoVehiculoService tipoVehiculoService,
        IVehiculoService vehiculoService,
        ITarifaService tarifaService,
        IConversacionesService conversacionesService,
        IRegionService regionService,
        IPlantaService plantaService,
        IDepositoService depositosService) //SE INYECTA el servicio de depósitos en el constructor para poder usarlo en los métodos relacionados con depósitos
        {
                _logger = logger;
                this.empresasService = empresasService;
                this.conductorService = conductorService;
                this.tipoVehiculoService = tipoVehiculoService;
                this.vehiculoService = vehiculoService;
                this.tarifaService = tarifaService;
                this.conversacionesService = conversacionesService;
                this.regionesService = regionService;
                this.plantaService = plantaService;
                this.depositosService = depositosService;  //SE ASIGNA el servicio de depósitos
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

        // =========== Region ==============
        public async Task<IActionResult> Regiones()
        {
            var regiones = await regionesService.GetRegiones(User.GetEmpresaId());
            return View(regiones);
        }

        // Region 260626
        //Metodo nuevo
        /// <summary>
        /// Muestra el formulario para agregar o editar una región.
        /// El ViewModel se construye en el servicio, filtrando la región por la empresa del usuario para respetar la seguridad jerárquica.
        /// </summary>
        /// <param name="id">Identificador de la región a editar; 0 para nueva.</param>
        public async Task<IActionResult> AgregarRegion(int id)
        {
            var agregarModel = await regionesService.GetRegionViewModel(User.GetEmpresaId(), id);
            return View(agregarModel);
        }

        /// <summary>
        /// Procesa el formulario de agregar/editar de una región.
        /// Pasa el identificador de empresa del usuario autenticado al servicio validando que la operación quede restringida a su empresa.
        /// </summary>
        /// <param name="model">Datos capturados en el formulario.</param>
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
                    return RedirectToAction("Regiones", "Configuracion");
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

        /// <summary>
        /// Muestra el listado de plantas de la empresa del usuario autenticado.
        /// Carga la navegación a Región para mostrar el nombre de región en la tabla.
        /// </summary>
        public async Task<IActionResult> Plantas()
        {
            var plantas = await plantaService.GetPlantas(User.GetEmpresaId());
            return View(plantas);
        }

        /// <summary>
        /// Muestra el formulario para agregar o editar una planta.
        /// El selector de Región se filtra por la empresa del usuario.
        /// </summary>
        /// <param name="id">Identificador de la planta a editar; 0 para nueva.</param>
        public async Task<IActionResult> AgregarPlanta(int id)
        {
            var model = await plantaService.GetPlantaViewModel(User.GetEmpresaId(), id);
            return View(model);
        }

        /// <summary>
        /// Procesa el formulario de agregar/editar de una planta.
        /// Recarga las regiones del selector si el modelo no es válido o hay excepción.
        /// </summary>
        /// <param name="model">Datos capturados en el formulario.</param>
        [HttpPost]
        public async Task<IActionResult> AgregarPlanta(PlantaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recarga la lista de regiones antes de devolver la vista
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
                    return RedirectToAction("Plantas", "Configuracion");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                // Recarga la lista de regiones antes de devolver la vista con el error
                var recargar = await plantaService.GetPlantaViewModel(User.GetEmpresaId(), model.IdPlanta);
                model.Regiones = recargar.Regiones;
                return View(model);
            }
        }
        // ============== Planta FIN ==============

        //Deposito
        public async Task<IActionResult> Depositos()
        {
            var depositos = await depositosService.GetDepositos(User.GetEmpresaId());
            return View(depositos);
        }


        public async Task<IActionResult> AgregarDeposito(int id)
        {
            var agregarModel = await depositosService.GetDepositoViewModel(User.GetEmpresaId(), id);

            //llena listas de regiones, plantas y zonas
            agregarModel.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());
            agregarModel.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());
            agregarModel.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());

            return View(agregarModel);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarDeposito(DepositoViewModel model)
        {
            ////forzar a que ignore listas ya que no vienen de regreso del html
            //ModelState.Remove("ListadoRegiones");
            //ModelState.Remove("ListadoPantas");
            //ModelState.Remove("ListadoZonas");

            try
            {
                //Si el formulario falla (Model.State.IsValid es false)
                //se deben recargar catalogos de lo contrario se veran vacios y la pagina se rompe al intentrar pintarlos
                if (!ModelState.IsValid)
                {
                    model.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());//se agrega regiones
                    model.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());//se agrega plantas
                    model.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());//se agrega zonas

                    return View(model);
                }

                var result = await depositosService.AgregarDeposito(model);

                if (result)
                {
                    return RedirectToAction("Depositos", "Configuracion");
                }


                // recarga de catalogos en caso de que servicio devuelva false
                model.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());//se agrega regiones
                model.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());//se agrega plantas
                model.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());//se agrega zonas

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                // recarga de catalogos en caso de que el servicio lance una excepcion
                model.ListadoRegiones = await depositosService.GetRegions(User.GetEmpresaId());//se agrega regiones
                model.ListadoPlantas = await depositosService.GetPlantas(User.GetEmpresaId());//se agrega plantas
                model.ListadoZonas = await depositosService.GetZonas(User.GetEmpresaId());//se agrega zonas

                return View(model);
            }


        }



        //METODOS PARA COMBOS CASCADA (AJAX)
        [HttpGet]
        public async Task<JsonResult> ObtenerPlantas(int regionId)
        {
            //obtener Id de la empresa del usuario actual
            var idEmpresa = User.GetEmpresaId();

            //llama al servicio para traer solo las plantas de esa region específica
            var plantas = await depositosService.GetPlantasByRegion(idEmpresa, regionId);

            //devolver datos en formato JSON al navegador
            return Json(plantas);
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerZonas(int plantaId)
        {
            //obtener Id de la empresa
            var idEmpresa = User.GetEmpresaId();

            //llama al servicio para traer solo las zonas de esa planta específica
            var zonas = await depositosService.GetZonasByPlanta(idEmpresa, plantaId);

            //regresamos lista de zonas en formato JSON al navegador
            return Json(zonas);

        }


        // Depositos FIN




    }
}