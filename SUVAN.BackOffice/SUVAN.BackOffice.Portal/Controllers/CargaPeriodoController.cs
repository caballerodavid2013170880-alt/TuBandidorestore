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
    public class CargaPeriodoController : Controller
    {
        private readonly ILogger<CargaPeriodoController> _logger;
        private readonly ICargaPeriodoService _cargaPeriodoService;

        public CargaPeriodoController(ILogger<CargaPeriodoController> logger,
            ICargaPeriodoService cargaPeriodoService)
        {
            _logger = logger;
            _cargaPeriodoService = cargaPeriodoService;
        }

        public IActionResult Index ()
        {
            return RedirectToAction("CargasPeriodo");
        }


        /// <summary>
        /// Muestra la pantalla principal de Cargas del periodo cargando el primer nivel de regiones
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CargasPeriodo()
        {
            var idEmpresa = User.GetEmpresaId();
            ViewBag.Regiones = await _cargaPeriodoService.GetRegions(idEmpresa);
            return View();
        }

        // combos en cascada (misma lógica AJAX depositos)

        [HttpGet]
        public async Task<JsonResult> GetPlantas(int regionId)
        {
            var idEmpresa = User.GetEmpresaId();
            var plantas = await _cargaPeriodoService.GetPlantasByRegion(idEmpresa, regionId);
            return Json(plantas);
        }

        [HttpGet]
        public async Task<JsonResult> GetZonas(int plantaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var zonas = await _cargaPeriodoService.GetZonasByPlanta(idEmpresa, plantaId);
            return Json(zonas);
        }

        [HttpGet]
        public async Task<JsonResult> GetDepositos(int zonaId)
        {
            var idEmpresa = User.GetEmpresaId();
            var depositos = await _cargaPeriodoService.GetDepositosByZona(idEmpresa, zonaId);
            return Json(depositos);
        }


        //datos de la tabla y estadisticas

        /// <summary>
        /// Obtiene las cargas del periodo asociadas al deposito seleccionado mapeadas para la tabla
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetCargasPeriodo (int idDeposito)
        {
            var idEmpresa = User.GetEmpresaId();
            var cargas = await _cargaPeriodoService.GetCargasPeriodo(idEmpresa, idDeposito);

            var resultado = new List<object>();
            foreach (var c in cargas)
            {
                resultado.Add(new
                {
                    //idCarga = c.IdCarga,
                    vehiculoEconomico = c.IdVehiculoNavigation?.Numeroeconomico ?? "",
                    marca = c.IdVehiculoNavigation?.Marca ?? "",
                    modelo = c.IdVehiculoNavigation?.Modelo ?? "",
                    placas = c.IdVehiculoNavigation?.Placas ?? "",
                    fecha = c.Fecha.ToString("dd/MM/yyyy"),
                    //hora = c.Hora.ToString("HH:mm"),                    
                    kmAnt = c.KmAnterior,
                    kmAct = c.KmActual,
                    kmRec = c.KmRecorridos,
                    nota = c.FolioNota,
                    litros = c.Litros,
                    costoXLt = c.CostoXLt,
                    importe = c.Importe,
                    rendimiento = c.Rendimiento,
                    espec = c.Espec,
                    combustible = c.IdCombNavigation?.Nombre ?? c.IdComb.ToString()                
                    //traspasar = c.Traspasar
                });
            }
            return Json(resultado);
        }

        /// <summary>
        /// Obtiene el bloque de estadisticas para el panel inferior
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetEstadisticas (int idDeposito)
        {
            var idEmpresa = User.GetEmpresaId();
            var estadisticas = await _cargaPeriodoService.GetEstadisticasPeriodo(idEmpresa, idDeposito);
            return Json(estadisticas);
        }
    }
}

