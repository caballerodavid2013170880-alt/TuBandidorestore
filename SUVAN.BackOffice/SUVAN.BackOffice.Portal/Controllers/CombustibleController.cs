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
                    hora = c.Hora.ToString("HH:MM"),
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
                        return View("AgregarCarga");
                    }
                }
                else
                {
                    ViewBag.Error = "El número de vehículo económico es requerido.";
                    ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
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
                    return View("AgregarCarga");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar carga manual: {ex.Message}");
                ViewBag.Error = "Error al procesar la carga: " + ex.Message;
                ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
                return View("AgregarCarga");
            }
        }



        //[HttpPost]
        //public async Task<IActionResult> SaveCargas(CombCarga nuevaCarga)
        //{
        //    try
        //    {
        //        // 1. Datos de auditoría obligatorios para la base de datos
        //        nuevaCarga.Idempresa = User.GetEmpresaId();
        //        nuevaCarga.Idusuario = User.GetUserId();
        //        nuevaCarga.Fecharegistro = DateTime.Now;

        //        // Si tu base de datos no acepta nulos en campos calculados, aseguramos ceros o falses iniciales
        //        nuevaCarga.Traspasar = (sbyte)(Request.Form["Traspasar"] == "1" ? 1 : 0);

        //        // 2. Mandamos el objeto en una lista al servicio ya existente que espera una colección
        //        var listaCargas = new List<CombCarga> { nuevaCarga };
        //        var exito = await cargasService.SaveCargasBD(listaCargas);

        //        // 3. Al ser un envío de formulario nativo, redireccionamos de vuelta al listado con un aviso
        //        if (exito)
        //        {
        //            return RedirectToAction("CargasTransitorias");
        //        }
        //        else
        //        {
        //            ViewBag.Error = "Ocurrió un error al guardar la carga en la base de datos.";
        //            ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
        //            return View("AgregarCarga");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error al guardar carga manual: {ex.Message}");
        //        ViewBag.Error = "Error al procesar la carga: " + ex.Message;
        //        ViewBag.Regiones = await cargasService.GetRegions(User.GetEmpresaId());
        //        return View("AgregarCarga");
        //    }
        //}


        //[HttpPost]
        //public async Task<JsonResult> SaveCargas([FromBody] List<CombCarga> cargasModificadas)
        //{
        //    try
        //    {
        //        //datos de auditoria antes de mandar al servicio
        //        var idUsuario = User.GetUserId();
        //        foreach (var c in cargasModificadas)
        //        {
        //            c.Idempresa = User.GetEmpresaId();
        //            c.Idusuario = idUsuario;
        //            c.Fecharegistro = DateTime.Now;
        //        }

        //        var exito = await cargasService.SaveCargasBD(cargasModificadas);
        //        return Json(new { success = exito, message = exito ? "Información guardada correctamente." : "Ocurrió un error al guardar." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error al procesar la carga: " + ex.Message });
        //    }
        //}




        /// <summary>
        /// Muestra la pantalla para la captura o registro de una carga manual
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AgregarCarga()
        {
            var idEmpresa = User.GetEmpresaId();
            // Cargamos las regiones en el ViewBag para alimentar el primer combo en cascada del formulario manual
            ViewBag.Regiones = await cargasService.GetRegions(idEmpresa);
            return View();
        }
    }
}




////Test
//namespace SUVAN.BackOffice.Portal.Controllers
//{
//    public class CombustibleController : Controller
//    {
//        public IActionResult Index()
//        {
//            return RedirectToAction("CargasTransitorias");
//        }

//        public async Task<IActionResult> CargasTransitorias()
//        {
//            // Simulación rápida de regiones para que pinte el primer combo
//            ViewBag.Regiones = new List<dynamic>
//            {
//                new { Id = 1, Nombre = "Región Norte" },
//                new { Id = 2, Nombre = "Región Centro" },
//                new { Id = 3, Nombre = "Región Sur" }
//            };
//            return View();
//        }

//        // 1. Carga de Plantas en Cascada
//        [HttpGet]
//        public IActionResult ObtenerPlantas(int regionId)
//        {
//            var plantas = new List<dynamic>
//            {
//                new { idPlanta = 101, nombrePlanta = $"Planta Alfa (R{regionId})" },
//                new { idPlanta = 102, nombrePlanta = $"Planta Beta (R{regionId})" }
//            };
//            return Json(plantas);
//        }

//        // 2. Carga de Zonas en Cascada
//        [HttpGet]
//        public IActionResult ObtenerZonas(int plantaId)
//        {
//            var zonas = new List<dynamic>
//            {
//                new { idZona = 201, nombreZona = $"Zona Metropolitana ({plantaId})" },
//                new { idZona = 202, nombreZona = $"Zona Foránea ({plantaId})" }
//            };
//            return Json(zonas);
//        }

//        // 3. Carga de Depósitos en Cascada
//        [HttpGet]
//        public IActionResult ObtenerDepositos(int zonaId)
//        {
//            var depositos = new List<dynamic>
//            {
//                new { idDeposito = 301, nombreDeposito = $"Depósito Central ({zonaId})" },
//                new { idDeposito = 302, nombreDeposito = $"Depósito Auxiliar ({zonaId})" }
//            };
//            return Json(depositos);
//        }

//        // 4. Carga de datos mock para la Cuadrícula (Pinta renglones válidos e inválidos)
//        [HttpGet]
//        public IActionResult ObtenerCargas(int idDeposito)
//        {
//            var listaMock = new List<dynamic>
//            {
//                // Un vehículo que SÍ va a existir (3066)
//                new {
//                    idCarga = 1, vehiculoEconomico = "3066", fecha = "2026-03-27", hora = "08:41",
//                    folioNota = "827", importe = 617.60, kmActual = 63849, kmAnterior = 63848,
//                    kmRecorridos = 1, litros = 80.00, rendimiento = "0.01", costoXLt = 7.72, traspasar = 0
//                },
//                // Un vehículo que NO va a existir para forzar el renglón ROJO (7001-NV)
//                new {
//                    idCarga = 2, vehiculoEconomico = "7001-NV", fecha = "2026-03-27", hora = "13:24",
//                    folioNota = "897", importe = 1144.50, kmActual = 981140, kmAnterior = 973578,
//                    kmRecorridos = 7562, litros = 150.00, rendimiento = "50.41", costoXLt = 7.63, traspasar = 1
//                }
//            };
//            return Json(listaMock);
//        }

//        // 5. Validación de Vehículo en tiempo real
//        [HttpGet]
//        public IActionResult ValidarVehiculo(string numeroEconomico)
//        {
//            // Simulamos que el "3066" sí existe en Kratos, cualquier otro pintará error
//            if (numeroEconomico == "3066")
//            {
//                return Json(new { existe = true, idVehiculo = 1542 });
//            }
//            return Json(new { existe = false, idVehiculo = 0 });
//        }

//        // 6. Recepción del Botón "Cargar a BASE"
//        [HttpPost]
//        public IActionResult GuardarCargas([FromBody] List<dynamic> cargas)
//        {
//            // Aquí llegará el JSON estructurado desde JavaScript cuando le den "Aceptar" al SweetAlert2
//            return Json(new { success = true, message = "Datos procesados correctamente en el servidor." });
//        }
//    }
//}


