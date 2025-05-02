using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Service.Viajes;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ViajesController : ControllerBase
  {
    private readonly ISuVanResponseService _suVanResponseService;
    private readonly IViajeService _viajeService;

    public ViajesController(ISuVanResponseService suVanResponseService, IViajeService viajeService)
    {
      _viajeService = viajeService;
      _suVanResponseService = suVanResponseService;
    }

    [HttpPost]
    [Route("BuscaServicio")]
    [SwaggerOperation(Description = "Este servicio busca los servicios para la ruta")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajeServicioResponse>>))]
    public async Task<ActionResult> BuscaServicio([FromBody] ViajeServicioRequest data)
    {
      try
      {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
        var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

        var response = await _viajeService.BuscaServicio(int.Parse(resultClaim ?? "0"), resultClaimEmail, data);
        if (response.CodigoMensaje != "200")
        {
          return _suVanResponseService.Handle(response);
        }

        return Ok(response);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpPost]
    [Route("BuscaDisponibilidad")]
    [SwaggerOperation(Description = "Este servicio busca disponibilidad en una ruta para los diferentes horarios y de acuerdo a la cantidad de pasajeros")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajeDisponibilidadResponse>>))]
    public async Task<ActionResult> BuscaDisponibilidad([FromBody] ViajeDisponibilidadRequest data)
    {
      try
      {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
        var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

        var response = await _viajeService.BuscaDisponibilidad(int.Parse(resultClaim ?? "0"), resultClaimEmail, data);
        if (response.CodigoMensaje != "200")
        {
          return _suVanResponseService.Handle(response);
        }

        return Ok(response);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpPost]
    [Route("ApartaReservacion")]
    [SwaggerOperation(Description = "Este servicio aparta la reservación para el viaje")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<ReservacionViajeReponse>))]
    public async Task<ActionResult> ApartaReservacion([FromBody] ReservacionViajeRequest data)
    {

      var usuario = getUser();
      var response = await _viajeService.ApartaReservacion(data, usuario);
      return _suVanResponseService.Handle(response);


    }

    [HttpPost]
    [Route("ObtenBoleto")]
    [SwaggerOperation(Description = "Este servicio obtiene la información y boletos del viaje")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<BoletoViajeResponse>))]
    public async Task<ActionResult> ObtenBoleto([FromBody] BoletoViajeRequest data)
    {
      try
      {
        var response = await _viajeService.ObtenBoleto(data);
        if (response.CodigoMensaje != "200")
        {
          return _suVanResponseService.Handle(response);
        }

        return Ok(response);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpGet]
    [Route("ObtenBoletoYProximosViaje")]
    [SwaggerOperation(Description = "Este servicio obtiene la información y boletos de los proximos viajes")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<BoletoViajeResponse>>))]
    public async Task<ActionResult> ObtenBoletoYProximosViaje()
    {
      try
      {
        var usuario = getUser();
        var response = await _viajeService.ObtenBoletoOffline(usuario);
        if (response.CodigoMensaje != "200")
        {
          return _suVanResponseService.Handle(response);
        }

        return Ok(response);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }


    [HttpPost]
    [Route("ObtenRecorrido")]
    [SwaggerOperation(Description = "Este servicio obtiene la información de las estaciones que tendra el recorrido")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<RecorridoViajeResponse>>))]
    public async Task<ActionResult> ObtenRecorrido([FromBody] RecorridoViajeRequest data)
    {
      var response = await _viajeService.ObtenRecorrido(data);
      return _suVanResponseService.Handle(response);
    }

    [HttpGet]
    [Route("ViajesFrecuentes")]
    [SwaggerOperation(Description = "Servicio que regresa los dos registros mas frecuentes del usuario")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajeRutaModels>>))]
    public async Task<ActionResult> ViajesFrecuentes()
    {
      var usuario = getUser();
      var response = await _viajeService.ViajesFrecuentes(usuario);
      return _suVanResponseService.Handle(response);
    }

    [HttpGet]
    [Route("ViajeCurso")]
    [SwaggerOperation(Description = "Servicio que regresa el viaje en curso")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<ViajeRutaResponse>))]
    public async Task<ActionResult> ViajeCurso()
    {
      var usuario = getUser();
      var response = await _viajeService.ViajeCurso(usuario, 1);
      return _suVanResponseService.Handle(response);
    }


    [HttpGet]
    [Route("ProximosViajes")]
    [SwaggerOperation(Description = "Servicio que regresa los proximos dos viajes del usuario")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajeRutaResponse>>))]
    public async Task<ActionResult> ProximosViajes()
    {
      var usuario = getUser();
      var response = await _viajeService.ProximosViajes(usuario, 2);
      return _suVanResponseService.Handle(response);
    }

    [HttpGet]
    [Route("ProximosViajesCompleto")]
    [SwaggerOperation(Description = "Servicio que regresa todos los proximos viajes del usuario")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajeRutaResponse>>))]
    public async Task<ActionResult> ProximosViajesCompleto()
    {
      var usuario = getUser();
      var response = await _viajeService.ProximosViajes(usuario, 0);
      return _suVanResponseService.Handle(response);
    }

    [HttpGet]
    [Route("ViajesAnteriores")]
    [SwaggerOperation(Description = "Servicio que regresa los viajes anteriores del usuario")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajeRutaResponse>>))]
    public async Task<ActionResult> ViajesAnteriores()
    {
      var usuario = getUser();
      var response = await _viajeService.ViajesAnteriores(usuario);
      return _suVanResponseService.Handle(response);
    }

    [HttpPost]
    [Route("Cancelacion")]
    [SwaggerOperation(Description = "Servicio para realizar la cancelación de un viaje")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<CancelacionViaje>))]
    public async Task<ActionResult> Cancelacion([FromBody] CancelaViajeRequest data)
    {
      var usuario = getUser();
      var response = await _viajeService.Cancelacion(usuario, data);
      return _suVanResponseService.Handle(response);
    }

    [HttpPost]
    [Route("BuscaFechasRuta")]
    [SwaggerOperation(Description = "Este servicio busca las fechas que tiene asignada una ruta")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ViajesServicioFechaResponse>>))]
    public async Task<ActionResult> BuscaFechasRuta([FromBody] ViajeFechasRequest data)
    {
      try
      {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
        var resultClaimEmail = identity.Claims.Where(x => x.Type == "email").Select(x => x.Value).FirstOrDefault();

        var response = await _viajeService.BuscaFechasRuta(data);
        if (response.CodigoMensaje != "200")
        {
          return _suVanResponseService.Handle(response);
        }

        return Ok(response);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }


    private int getUser()
    {
      var identity = HttpContext.User.Identity as ClaimsIdentity;
      if (identity != null)
      {
        var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
        return int.Parse(resultClaim);
      }

      return 0;
    }

  }
}
