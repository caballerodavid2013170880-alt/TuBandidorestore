using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ViewModel.Logistica;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Logistica;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Portal.Controllers
{
    [Authorize]
    public class CombustibleController : Controller
    {
        private readonly ILogger<CombustibleController> _logger;
        private readonly ICargasTransitoriasService cargasService;

        public CombustibleController(ILogger<CombustibleController> logger,
            ICargasTransitoriasService cargasService)
        {
            _logger = logger;
            this.cargasService = cargasService;
        }

        public IActionResult Index ()
        {
            return RedirectToAction("CargasTransitorias");
        }


        /// <summary>
        /// Muestra la pantalla principal de Cargas Transitorias cargando el primer nivel de regionas
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CargasTransitorias()
        {
            var idEmpresa = User.GetEmpresaId();
            ViewBag.Regiones = await cargasService.GetRegions(idEmpresa);
            return View();
        }

        // combos en cascada (misma lógica AJAX depositos)

        [HttpGet]
        public async Task<JsonResult> GetPlantas(int regionId)
        {
            var idEmpresa = User.GetEmpresaId();
            var plantas = await cargasService.GetPlantasByRegion(idEmpresa, regionId);
            return Json(plantas);
        }

        [HttpGet]
        public async Task<JsonResult> GetZonas(int plantaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var zonas = await cargasService.GetZonasByPlanta(idEmpresa, plantaId);
            return Json(zonas);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepositos(int zonaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var depositos = await cargasService.GetDepositosByZona(idEmpresa, zonaId);
            return Json(depositos);
        }


        /// <summary>
        /// Obtiene las cargas transitorias asociadas al deposito seleccionado
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetCargas(int idDeposito)
        {
            var idEmpresa = User.GetEmpresaId();
            var cargas = await cargasService.GetCargasTransitorias(idEmpresa, idDeposito);

            var resultado = new List<object>();
            foreach (var c in cargas)
            {
                resultado.Add(new
                {
                    idCarga = c.IdCarga,
                    vehiculoEconomico = c.IdVehiculoNavigation?.Numeroeconomico ?? "",
                    fecha = c.Fecha.ToString("yyyy/MM/dd"),
                    hora = c.Hora.ToString("HH:mm"),
                    folioNota = c.FolioNota,
                    importe = c.Importe,
                    litros = c.Litros,
                    kmAnterior = c.KmAnterior,
                    kmActual = c.KmActual,
                    kmRecorridos = c.KmRecorridos,
                    rendimiento = c.Rendimiento,
                    costoXLt = c.CostoXLt,
                    traspasar = c.Traspasar
                });
            }
            return Json(resultado);
        }

        /// <summary>
        /// Valida si el numero economico existe en la BD para la empresa actual
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> ValidarVehiculo(string numeroEconomico)
        {
            var idEmpresa = User.GetEmpresaId();
            var idVehiculo = await cargasService.GetIdVehiculoByEconomico(numeroEconomico, idEmpresa);
            return Json(new { existe = idVehiculo.HasValue, idVehiculo = idVehiculo ?? 0 });
        }

        /// <summary>
        /// Procesa el guardado a base enviado desde formulario
        /// </summary>

        /// <summary>
        /// Procesa el guardado a base enviado desde formulario
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveCargas(CombCarga nuevaCarga)
        {
            try
            {
                // 1. Datos de auditoría obligatorios para la base de datos
                nuevaCarga.Idempresa = User.GetEmpresaId();
                nuevaCarga.Idusuario = User.GetUserId();
                nuevaCarga.Fecharegistro = DateTime.Now;

                nuevaCarga.Idfactura = (nuevaCarga.Idfactura <= 0) ? null : nuevaCarga.Idfactura;

                nuevaCarga.IdDepto = (nuevaCarga.IdDepto <= 0) ? null : nuevaCarga.IdDepto;

                //nuevaCarga.Espec =string.IsNullOrEmpty(nuevaCarga.Espec) ? "" : nuevaCarga.Espec;

                //Corrección evita error de llave forane en factura 
                if (nuevaCarga.Idfactura <= 0)
                {
                    nuevaCarga.Idfactura = null;

                }

                //Asignacion de un valor por defecto a espec para evitar errores de null en la base de datos
                nuevaCarga.Espec = string.IsNullOrEmpty(nuevaCarga.Espec) ? "" : nuevaCarga.Espec;

                // Recuperamos el valor del checkbox de traspasar
                nuevaCarga.Traspasar = (sbyte)(Request.Form["Traspasar"] == "1" ? 1 : 0);

                // Obtenemos el número económico enviado desde el HTML y buscamos su IdVehiculo real
                string numEconomico = Request.Form["VehiculoEconomico"];
                if (!string.IsNullOrEmpty(numEconomico))
                {
                    var idVehiculo = await cargasService.GetIdVehiculoByEconomico(numEconomico, nuevaCarga.Idempresa);
                    if (idVehiculo.HasValue)
                    {
                        nuevaCarga.IdVehiculo = idVehiculo.Value;
                    }
                    else
                    {
                        // Si el vehículo no existe, rompemos el flujo y avisamos al usuario
                        ViewBag.Error = $"El vehículo económico '{numEconomico}' no existe en el sistema.";
                        ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
                        ViewBag.TiposCombustible = await cargasService.GetTiposCombustible();
                        return View("AgregarCarga");
                    }
                }
                else
                {
                    ViewBag.Error = "El número de vehículo económico es requerido.";
                    ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
                    ViewBag.TiposCombustible = await cargasService.GetTiposCombustible();
                    return View("AgregarCarga");
                }

                // 2. Mandamos el objeto en una lista al servicio ya existente que espera una colección
                var listaCargas = new List<CombCarga> { nuevaCarga };
                var exito = await cargasService.SaveCargasBD(listaCargas);

                // 3. Al ser un envío de formulario nativo, redireccionamos de vuelta al listado con un aviso
                if (exito)
                {
                    return RedirectToAction("CargasTransitorias");
                }
                else
                {
                    ViewBag.Error = "Ocurrió un error en las reglas de negocio al guardar la carga.";
                    ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
                    ViewBag.TiposCombustible = await cargasService.GetTiposCombustible();
                    return View("AgregarCarga");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar carga manual: {ex.Message}");
                ViewBag.Error = "Error al procesar la carga: " + ex.Message;
                ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
                ViewBag.TiposCombustible = await cargasService.GetTiposCombustible();
                return View("AgregarCarga");
            }
        }


        /// <summary>
        /// Muestra la pantalla para la captura o registro de una carga manual
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AgregarCarga()
        {
            var idEmpresa = User.GetEmpresaId();
            // Cargamos las regiones en el ViewBag para alimentar el primer combo en cascada del formulario manual
            ViewBag.Regiones = await cargasService.GetRegions(idEmpresa);
            //Carga los tipos de combustible
            ViewBag.TiposCombustible = await cargasService.GetTiposCombustible();
            return View();
        }
    }
}

