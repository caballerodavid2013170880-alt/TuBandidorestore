using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Text.Json;
using SUVAN.BackOffice.Models.ViewModel;
using SUVAN.BackOffice.Models.ViewModel.Configuracion;
using SUVAN.BackOffice.Portal.Helper;
using SUVAN.BackOffice.Service.Configuracion;
using SUVAN.BackOffice.Service.Monitoreo;
using SUVAN.BackOffice.Database.Entities;
using System.Security.Claims;

namespace SUVAN.BackOffice.Portal.Controllers
{

  [Authorize]
  public class MonitoreoController : Controller
  {
    private readonly ILogger<MonitoreoController> logger;
    private readonly IRastreoService rastreoService;
    private readonly ICorridasService corridasService;
    private readonly IRutasService rutasService;

    public MonitoreoController(ILogger<MonitoreoController> logger, IRastreoService rastreoService, ICorridasService corridasService, IRutasService rutasService)
    {
      this.logger = logger;
      this.rastreoService = rastreoService;
      this.corridasService = corridasService;
      this.rutasService = rutasService;
    }

    private async Task<RastreoJSONModel> GetData()
    {
      var identity = HttpContext.User.Identity as ClaimsIdentity;
      var vEmpresa = identity.Claims.Where(x => x.Type == "Empresa").Select(x => x.Value).FirstOrDefault();

      RastreoJSONModel response = new RastreoJSONModel();
      var CorrDataJSON = new List<CorridasAsignadasJSONModel>();
      var RutaDataJSON = new List<RutasJSONModel>();
      var corridasAsig = await rastreoService.GetCorridas(int.Parse(vEmpresa));
      foreach (var idRuta in corridasAsig.Select(x => x.CorridaIdcorridaNavigation.RutaIdruta).Distinct())
      {
        var ruta = await rastreoService.GetRuta(idRuta);
        var lParada = new List<RutaParadaJSONModel>();

        foreach (var rp in ruta.RutaParada.Where(x => x.Activo == 1).OrderBy(x => x.Orden))
        {
          //var vParada = await rastreoService.GetParada(rp.ParadaIdparada);
          lParada.Add(new RutaParadaJSONModel
          {
            IdParada = rp.ParadaIdparada,
            Nombre = rp.ParadaIdparadaNavigation.Nombre,
            Latitude = rp.ParadaIdparadaNavigation.Latitud ?? 0,
            Longitude = rp.ParadaIdparadaNavigation.Longitud ?? 0
          });
        }

        RutaDataJSON.Add(new RutasJSONModel
        {
          Idruta = ruta.Idruta,
          Nombre = ruta.Nombre,
          GeoRuta = ruta.Googlemapsruta,
          Paradas = lParada
        });
      }

      foreach (var corrida in corridasAsig)
      {
        CorrDataJSON.Add(new CorridasAsignadasJSONModel
        {
          IdcorridaAsignacion = corrida.IdcorridaAsignacion,
          IdVehiculo = corrida.VehiculoIdvehiculoNavigation.Idvehiculo,
          Placa = corrida.VehiculoIdvehiculoNavigation.Placas,
          IdConductor = corrida.ConductorIdconductorNavigation.Idconductor,
          NombreConductor = corrida.ConductorIdconductorNavigation.Nombre,
          /*Latitude = corrida.CurrentLat ?? 0,
          Longitude = corrida.CurrentLong ?? 0,*/
          IdRuta = corrida.CorridaIdcorridaNavigation.RutaIdruta
        });
      }

      response.Corridas = CorrDataJSON;
      response.Rutas = RutaDataJSON.OrderBy(x => x.Idruta).ToList();

      return response;
    }

    public async Task<IActionResult> Rastreo()
    {
      var response = await GetData();
      ViewBag.Corridas = "";
      ViewBag.CorridasJSON = JsonSerializer.Serialize(response.Corridas);
      ViewBag.Rutas = JsonSerializer.Serialize(response.Rutas);

      return View();
    }

    public async Task<IActionResult> RastreoUpdate()
    {
      var response = await GetData();
      return Json(response, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }
  }
}
