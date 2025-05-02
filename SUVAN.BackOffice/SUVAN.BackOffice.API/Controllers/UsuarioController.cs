using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Service;
using System;
using System.Net;
using System.Runtime.Serialization;
using SUVAN.BackOffice.Models.Errores;
using SUVAN.BackOffice.API.Controllers;
using SUVAN.BackOffice.Models.RegistroUsuario;
using SUVAN.BackOffice.Models.GeneraCodigo;
using SUVAN.BackOffice.Service.RegistroUsuario;
using SUVAN.BackOffice.Models.ActivacionUsuario;
using SUVAN.BackOffice.Models.RecuperaPassword;
using SUVAN.BackOffice.Models.ActualizaPassword;
using Swashbuckle.AspNetCore.Annotations;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.API.Provider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SUVAN.BackOffice.Service.Notificaciones;
using SUVAN.BackOffice.Utilities;
using SUVAN.BackOffice.Service.Seguridad;
using Microsoft.AspNetCore.Http;
using SUVAN.BackOffice.Models.Favoritos;
using System.Security.Claims;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.ObjectParentResponse;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Principal;
using SUVAN.BackOffice.Models.ActualizaFotografia;
using SUVAN.BackOffice.Models.Viajes;

namespace SUVAN.BackOffice.API
{
  [ApiController]
  [Route("Usuario")]

  public class UsuarioController : ControllerBase
  {
    private readonly ISuVanResponseService _suVanResponseService;
    private readonly IUsuarioService _usuario;
    private readonly INotificacionCorreoService _notificacionCorreoService;


    private readonly string? _codigoauth;
    public UsuarioController(ISuVanResponseService suVanResponseService, IUsuarioService registro, INotificacionCorreoService notificacionCorreoService)
    {
      _suVanResponseService = suVanResponseService;
      _usuario = registro;
      _notificacionCorreoService = notificacionCorreoService;
    }

    [HttpPost]
    [Route("Registro")]
    [SwaggerOperation(Description = "Servicio registro de usuarios")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> Registro([FromBody] RegistroUsuarioRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var usuarioResult = await _usuario.getInfoUsuario(data.email, "");
        if (usuarioResult != null)
        {
          return ResponseExcepciones(response, "400", "Ya existe una cuenta asociada a este correo");
        }
        string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
        response = await _usuario.Registro(data, code);
        #region Envio de Correo
        if (response.CodigoMensaje != "200")
        {
          return ResponseExcepciones(response, "400", response.Mensaje);
        }
        await _notificacionCorreoService.ActivacionCuenta(data.email!, data.nombre!, code, string.Empty, string.Empty);
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
    [Route("Activacion")]
    [SwaggerOperation(Description = "Servicio activación de usuarios")]
    [AllowAnonymous]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<string>))]
    public async Task<ActionResult> Activacion([FromBody] ActivacionUsuarioRequest data)
    {
      SuVanResponse<string> response = new();
      try
      {
        var usuarioResult = await _usuario.getInfoUsuario(data.email, data.password);
        if (usuarioResult == null)
        {
          return ResponseExcepciones(response, "400", "Email o password incorrectos");
        }
        if (usuarioResult.CodigoAuth != data.codigo)
        {
          return ResponseExcepciones(response, "400", "Código no válido");
        }
        if (DateTime.Now > usuarioResult.CodigoExp)
        {
          return ResponseExcepciones(response, "400", "Código expirado");
        }
        response = await _usuario.Activacion(data);
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
        var usuarioResult = await _usuario.getInfoUsuario(data.email, data.password);
        if (usuarioResult == null)
        {
          return ResponseExcepciones(response, "400", "Email o password incorrectos");
        }
        string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
        response = await _usuario.GeneraCodigo(data, code);
        #region Envio de Correo
        await _notificacionCorreoService.EnviaCodigo(data.email!, usuarioResult.Nombre, code, string.Empty, string.Empty);
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
        var usuarioResult = await _usuario.getInfoUsuario(data.email, string.Empty);
        if (usuarioResult == null)
        {
          return ResponseExcepciones(response, "400", "Email incorrecto");
        }
        response = await _usuario.RecuperaPassword(data);
        //#region Envio de Correo
        //await _notificacionCorreoService.InstruccionesRecuperaPassword(data.email!, usuarioResult.Nombre, string.Empty, string.Empty, string.Empty);
        //#endregion
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
        var usuarioResult = await _usuario.getInfoUsuario(data.email, string.Empty);
        if (usuarioResult == null)
        {
          return ResponseExcepciones(response, "400", "Email incorrecto");
        }
        response = await _usuario.ActualizaPassword(data);
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
    [SwaggerResponse(200, Type = typeof(SuVanResponse<PerfilUsuarioModel>))]
    public async Task<ActionResult> ObtenerPerfil()
    {
      var identity = HttpContext.User.Identity as ClaimsIdentity;
      var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
      var response = await _usuario.ObtenerPerfil(int.Parse(resultClaim ?? "0"));
      return _suVanResponseService.Handle(response);
    }
    [HttpPut]
    [Route("ActualizaFotografia")]
    [SwaggerOperation(Description = "Este servicio únicamente actualiza la fotografía")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<PerfilModel>))]
    public async Task<ActionResult> ActualizaFotografia(ActualizaFotografiaRequest data)
    {
      var identity = HttpContext.User.Identity as ClaimsIdentity;
      var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();
      var response = await _usuario.ActualizaFotografia(int.Parse(resultClaim ?? "0"), data);
      return _suVanResponseService.Handle(response);
    }
    [HttpPut]
    [Route("ActualizaPerfil")]
    [SwaggerOperation(Description = "Este servicio se utiliza para que los usuarios registren una ubicación GPS (latitud,longitud) en donde el usuario pone el nombre de la ubicación")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<ActualizaPerfilRequest>))]
    public async Task<ActionResult> ActualizaPerfil(ActualizaPerfilRequest model)
    {
      var identity = HttpContext.User.Identity as ClaimsIdentity;
      var resultClaim = identity.Claims.Where(x => x.Type == "userId").Select(x => x.Value).FirstOrDefault();

      string code = await Utilities.GeneraCodigos.GetGeneraCodigo(9999);
      var response = await _usuario.ActualizaPerfil(int.Parse(resultClaim ?? "0"), model, code);
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

    [HttpPost]
    [Route("CalificaConductor")]
    [SwaggerOperation(Description = "Servicio para calificar al conductor")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<ViajeCalificacion>))]
    public async Task<ActionResult> CalificaConductor([FromBody] ViajeCalificacion data)
    {
      try
      {
        var response = await _usuario.CalificaConductor(data);
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
            var response = await _usuario.Firebase(idUsuario, firebaseid);
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


    }
}
