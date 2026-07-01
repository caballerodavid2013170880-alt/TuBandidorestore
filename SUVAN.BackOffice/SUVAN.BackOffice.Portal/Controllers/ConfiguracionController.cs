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
using SUVAN.BackOffice.Service.Administrativo;
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
        // Deptos
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
        IDepositoService depositosService,
        IDeptoService deptoService) //SE INYECTA el servicio de depósitos en el constructor para poder usarlo en los métodos relacionados con depósitos
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

        // ============== Departamentos ==============

        /// <summary>
        /// Muestra el listado de departamentos de la empresa del usuario autenticado.
        /// Carga la navegaciÃ³n a DepÃ³sito para mostrar el nombre del depÃ³sito en la tabla.
        /// </summary>
        public async Task<IActionResult> Depto()
        {
            var deptos = await deptoService.GetDepto(User.GetEmpresaId());
            return View(deptos);
        }

        /// <summary>
        /// Muestra el formulario para agregar o editar un departamento.
        /// En modo alta solo carga las regiones; el resto de selectores se cargan vÃ­a AJAX.
        /// En modo ediciÃ³n pre-carga los cuatro selectores desde el servidor.
        /// </summary>
        /// <param name="id">Identificador del departamento a editar; 0 para nuevo.</param>
        public async Task<IActionResult> AgregarDepto(int id)
        {
            var model = await deptoService.GetDeptoViewModel(User.GetEmpresaId(), id);
            return View(model);
        }

        /// <summary>
        /// Procesa el formulario de agregar/editar de un departamento.
        /// Valida la jerarquÃ­a completa (Empresa â†’ RegiÃ³n â†’ Planta â†’ Zona â†’ DepÃ³sito)
        /// y recarga las listas de selectores si la validaciÃ³n falla.
        /// </summary>
        /// <param name="model">Datos capturados en el formulario.</param>
        [HttpPost]
        public async Task<IActionResult> AgregarDepto(DeptoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recarga las listas de selectores antes de devolver la vista
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
                    return RedirectToAction("Depto", "Configuracion");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                // Recarga las listas de selectores antes de devolver la vista con el error
                var recargar = await deptoService.GetDeptoViewModel(User.GetEmpresaId(), model.IdDepto);
                model.Regiones = recargar.Regiones;
                model.Plantas = recargar.Plantas;
                model.Zonas = recargar.Zonas;
                model.Depositos = recargar.Depositos;
                model.CascadeJson = recargar.CascadeJson;
                return View(model);
            }
        }

        /*
        /// <summary>
        /// Elimina un departamento. Valida que pertenezca a la empresa del usuario
        /// antes de realizar la operación.
        /// </summary>
        /// <param name="model">Modelo con el <c>IdDepto</c> del departamento a eliminar.</param>
        [HttpPost]
        public async Task<IActionResult> EliminarDepto([FromBody] DeptoViewModel model)
        {
            try
            {
                await deptoService.EliminarDepto(model.IdDepto, User.GetEmpresaId());
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        */
        // ── Endpoints AJAX para la carga en cascada de los selectores ──

        /// <summary>
        /// Endpoint AJAX: devuelve las plantas disponibles para la región indicada,
        /// filtradas por la empresa del usuario autenticado.
        /// Consumido por el selector de Planta al cambiar la Región en el formulario.
        /// </summary>
        /// <param name="idRegion">Identificador de la región seleccionada.</param>
        /// <returns>JSON con la lista de plantas (<c>idPlanta</c>, <c>nombre</c>).</returns>
        [HttpGet]
        public async Task<IActionResult> GetPlantasPorRegion(int idRegion)
        {
            var plantas = await deptoService.GetPlantasPorRegion(User.GetEmpresaId(), idRegion);
            return Json(plantas);
        }

        /// <summary>
        /// Endpoint AJAX: devuelve las zonas disponibles para la región y planta indicadas,
        /// filtradas por la empresa del usuario autenticado.
        /// Consumido por el selector de Zona al cambiar la Planta en el formulario.
        /// </summary>
        /// <param name="idRegion">Identificador de la región actualmente seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta seleccionada.</param>
        /// <returns>JSON con la lista de zonas (<c>idZona</c>, <c>nombre</c>).</returns>
        [HttpGet]
        public async Task<IActionResult> GetZonasPorPlanta(int idRegion, int idPlanta)
        {
            var zonas = await deptoService.GetZonasPorPlanta(User.GetEmpresaId(), idRegion, idPlanta);
            return Json(zonas);
        }

        /// <summary>
        /// Endpoint AJAX: devuelve los depósitos disponibles para la región, planta y zona indicadas,
        /// filtrados por la empresa del usuario autenticado.
        /// Consumido por el selector de Depósito al cambiar la Zona en el formulario.
        /// </summary>
        /// <param name="idRegion">Identificador de la región actualmente seleccionada.</param>
        /// <param name="idPlanta">Identificador de la planta actualmente seleccionada.</param>
        /// <param name="idZona">Identificador de la zona seleccionada.</param>
        /// <returns>JSON con la lista de depósitos (<c>idDeposito</c>, <c>nombre</c>).</returns>
        [HttpGet]
        public async Task<IActionResult> GetDepositosPorZona(int idRegion, int idPlanta, int idZona)
        {
            var depositos = await deptoService.GetDepositosPorZona(User.GetEmpresaId(), idRegion, idPlanta, idZona);
            return Json(depositos);
        }

        // ============== Departamentos FIN ==============

    }
}