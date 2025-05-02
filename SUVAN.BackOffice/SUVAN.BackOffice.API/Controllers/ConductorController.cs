using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.ActivacionUsuario;
using SUVAN.BackOffice.Models.ActualizaFotografia;
using SUVAN.BackOffice.Models.ActualizaPassword;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.Conductor;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.RecuperaPassword;
using SUVAN.BackOffice.Models.RegistroUsuario;
using SUVAN.BackOffice.Models.Viajes;
using SUVAN.BackOffice.Service.Conductor;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Service.RegistroUsuario;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
  [ApiController]
  [Route("Conductor")]

  public class ConductorController : ControllerBase
  {
    private readonly ISuVanResponseService _suVanResponseService;
    private readonly IConductorService _conductor;
    private readonly INotificacionCorreoService _notificacionCorreoService;


    private readonly string? _codigoauth;
    public ConductorController(ISuVanResponseService suVanResponseService, IConductorService conductor, INotificacionCorreoService notificacionCorreoService)
    {
      _suVanResponseService = suVanResponseService;
      _conductor = conductor;
      _notificacionCorreoService = notificacionCorreoService;
    }

    [HttpPost]
    [Route("Registro")]
    [SwaggerOperation(Description = "Servicio registro de conductores")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> Registro([FromBody] ConductorRegistroRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var result = await _conductor.getInfo(data.email, "");
        if (result != null)
        {
          return ResponseExcepciones(response, "400", "Ya existe una cuenta asociada a este correo");
        }

        response = await _conductor.Registro(data);
        #region Envio de Correo
        if (response.CodigoMensaje != "200")
        {
          return ResponseExcepciones(response, "400", response.Mensaje);
        }
        #endregion
        return _suVanResponseService.Handle(response);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    private ActionResult ResponseExcepciones(SuVanResponse<string> response, string CodigoMensaje, string Mensaje)
    {
      response.CodigoMensaje = CodigoMensaje;
      response.Mensaje = Mensaje;
      return _suVanResponseService.Handle(response);
    }

    [HttpPost]
    [Route("SolicitaActivacion")]
    [SwaggerOperation(Description = "Servicio para generar el OTP de activación")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    [SwaggerResponse(400, "Si el correo no es valido o el usuario ya esta activo")]
    public async Task<ActionResult> SolicitaActivacion([FromBody] SolicitaActivacionRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var result = await _conductor.getInfoEmail(data.email);
        if (result == null)
        {
          return ResponseExcepciones(response, "400", "Email incorrecto");
        }
        else if (result.Validado == 1)
        {
          return ResponseExcepciones(response, "400", "El usuario ya esta activo");
        }

        response = await _conductor.SolicitaActivacion(data);
        #region Envio de Correo
        await _notificacionCorreoService.EnviaCodigo(data.email!, result.Nombre, result.CodigoAuth, string.Empty, string.Empty);
        #endregion

        return _suVanResponseService.Handle(response);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpPost]
    [Route("Activacion")]
    [SwaggerOperation(Description = "Servicio activación de conductores")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> Activacion([FromBody] ActivacionUsuarioRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var result = await _conductor.getInfoEmail(data.email);
        if (result == null)
        {
          return ResponseExcepciones(response, "400", "Email incorrecto");
        }
        if (result.CodigoAuth != data.codigo)
        {
          return ResponseExcepciones(response, "400", "Código no válido");
        }
        if (DateTime.Now > result.CodigoExp)
        {
          return ResponseExcepciones(response, "400", "Código expirado");
        }
        response = await _conductor.Activacion(data);
        return _suVanResponseService.Handle(response);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpPost]
    [Route("GeneraCodigo")]
    [SwaggerOperation(Description = "Servicio para generar código")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> GeneraCodigo([FromBody] GeneraCodigoRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var result = await _conductor.getInfoEmail(data.email);
        if (result == null)
        {
          return ResponseExcepciones(response, "400", "Email o password incorrectos");
        }
        string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
        response = await _conductor.GeneraCodigo(data);
        #region Envio de Correo
        await _notificacionCorreoService.EnviaCodigo(data.email!, result.Nombre, code, string.Empty, string.Empty);
        #endregion

        return _suVanResponseService.Handle(response);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }


    [HttpPost]
    [Route("RecuperaPassword")]
    [SwaggerOperation(Description = "Servicio envio de correo para cambiar password")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> RecuperaPassword([FromBody] RecuperaPasswordRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var result = await _conductor.getInfo(data.email, string.Empty);
        if (result == null)
        {
          return ResponseExcepciones(response, "400", "Email incorrecto");
        }
        response = await _conductor.RecuperaPassword(data);
        #region Envio de Correo
        await _notificacionCorreoService.InstruccionesRecuperaPassword(data.email!, result.Nombre, string.Empty, string.Empty, string.Empty);
        #endregion
        return _suVanResponseService.Handle(response);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpPut]
    [Route("ActualizaPassword")]
    [SwaggerOperation(Description = "Servicio actualización de password")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> ActualizaPassword([FromBody] ActualizaPasswordRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var result = await _conductor.getInfo(data.email, string.Empty);
        if (result == null)
        {
          return ResponseExcepciones(response, "400", "Email incorrecto");
        }
        response = await _conductor.ActualizaPassword(data);
        return _suVanResponseService.Handle(response);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }

    [HttpGet]
    [Route("ObtenerPerfil")]
    [SwaggerOperation(Description = "Este servicio regresa la información de perfil, es decir: Nombre, Teléfono, Correo, Fotografía ")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<PerfilModel>))]
    public async Task<ActionResult> ObtenerPerfil()
    {
      int conductor = getUser();

      var response = await _conductor.ObtenerPerfil(conductor);
      return _suVanResponseService.Handle(response);
    }


    [HttpPut]
    [Route("ActualizaFotografia")]
    [SwaggerOperation(Description = "Este servicio únicamente actualiza la fotografía")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<PerfilModel>))]
    public async Task<ActionResult> ActualizaFotografia(ActualizaFotografiaRequest data)
    {
      int conductor = getUser();

      var response = await _conductor.ActualizaFotografia(conductor, data);
      return _suVanResponseService.Handle(response);
    }
    [HttpPut]
    [Route("ActualizaPerfil")]
    [SwaggerOperation(Description = "Este servicio se utiliza para que los usuarios registren una ubicación GPS (latitud,longitud) en donde el usuario pone el nombre de la ubicación")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<ActualizaPerfilRequest>))]
    public async Task<ActionResult> ActualizaPerfil(ActualizaPerfilRequest model)
    {
      int conductor = getUser();

      string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
      var response = await _conductor.ActualizaPerfil(conductor, model, code);
      if (response.CodigoMensaje != "200" && response.CodigoMensaje != "206")
      {
        return _suVanResponseService.Handle(response);
      }
      #region Envio de Correo
      if (response.CodigoMensaje == "206" && response.Mensaje == "Solicitud de actualización exitosa")
      {
        await _notificacionCorreoService.SolicitudActualizaTelefono(response.Data.Email, response.Data.Nombre, code, string.Empty, string.Empty);
      }
      #endregion

      return _suVanResponseService.Handle(response);
    }

    [HttpGet]
    [Route("ProximasCorridas")]
    [SwaggerOperation(Description = "Servicio que regresa las proximas corridas de un conductor")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConductorCorridas>>))]
    //[AllowAnonymous]
    public async Task<ActionResult> ProximasCorridas()
    {
      var conductor = getUser();
      var response = await _conductor.ProximasCorridas(conductor);
      return _suVanResponseService.Handle(response);
    }

    [HttpGet]
    [Route("RutasCorridaAsignada")]
    [SwaggerOperation(Description = "Servicio que regresa las rutas en las que ha sido asignado un conductor")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<RutaCorridaAsignada>>))]
    public async Task<ActionResult> RutasCorridaAsignada()
    {
      var conductor = getUser();
      var response = await _conductor.RutasCorridaAsignada(conductor);
      return _suVanResponseService.Handle(response);
    }

    [HttpPost]
    [Route("BusquedaCorridasAsignadas")]
    [SwaggerOperation(Description = "Servicio que regresa las corridas de un conductor")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConductorCorridas>>))]
    public async Task<ActionResult> BusquedaCorridasAsignadas(BusquedaCorridasRequest data)
    {
      var conductor = getUser();
      var response = await _conductor.BusquedaCorridasAsignadas(data, conductor);
      return _suVanResponseService.Handle(response);
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

    [HttpPost]
    [Route("CalificaCorrida")]
    [SwaggerOperation(Description = "Servicio para calificar a una corrida")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<CorridaCalificacion>))]
    public async Task<ActionResult> CalificaCorrida([FromBody] CorridaCalificacion data)
    {
      try
      {
        var response = await _conductor.CalificaCorrida(data);
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
    [Route("ConductorCalificaciones")]
    [SwaggerOperation(Description = "Este servicio regresa las calificaciones del conductor")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<CalificacionesViajeResponse>))]
    public async Task<ActionResult> ConductorCalificaciones(int corrida_asignacionId)
    {
      ///int conductor = getUser();
      var response = await _conductor.ConductorCalificaciones(corrida_asignacionId);
      return _suVanResponseService.Handle(response);
    }

    [HttpPost]
    [Route("Estadisticas")]
    [SwaggerOperation(Description = "Este servicio obtiene la información de las estadisticas del conductor")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<EstadisticasConductorResponse>))]
    public async Task<ActionResult> Estadisticas([FromBody] EstatidisticasConductorRequest data)
    {
      var userId = getUser();
      try
      {
        var response = await _conductor.EstadisticasConductor(userId, data);
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


    [HttpPut]
    [Route("Firebase")]
    [SwaggerOperation(Description = "Este servicio únicamente actualiza el firebase ID del usuario")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> Firebase(string firebaseid)
    {
      int idUsuario = getUser();
      var response = await _conductor.Firebase(idUsuario, firebaseid);
      return _suVanResponseService.Handle(response);
    }



  }
}
