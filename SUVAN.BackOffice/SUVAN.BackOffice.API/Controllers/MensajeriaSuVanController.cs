using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using SUVAN.BackOffice.API.Provider;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Auth.User;
using SUVAN.BackOffice.Models.ManejoRespuesta;
using SUVAN.BackOffice.Models.Mensajeria;
using SUVAN.BackOffice.Models.Mensajeria.Operador;
using SUVAN.BackOffice.Service.Conductor;
using SUVAN.BackOffice.Service.MensajeriaService;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace SUVAN.BackOffice.API.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class MensajeriaSuVanController : ControllerBase
  {
    private readonly ISuVanResponseService _suVanResponseService;
    private readonly IConductorService conductorService;
    private readonly IConversacionesService _conversacionesService;
    private readonly IHubContext<HubSuVanService, IHubSuVanService> _messageHub;

    public MensajeriaSuVanController(IConversacionesService conversacionesService
      , IHubContext<HubSuVanService, IHubSuVanService> messageHub
      , ISuVanResponseService suVanResponseService
      , IConductorService conductorService)
    {
      _conversacionesService = conversacionesService;
      _messageHub = messageHub;
      _suVanResponseService = suVanResponseService;
      this.conductorService = conductorService;
    }



    [HttpGet]
    [Route("ObtineAdministradoresConversacion")]
    [SwaggerOperation(Description = "Obtine listado de administradores para chat")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<AdministradorConversacionModel>>))]

    public async Task<ActionResult> ObtineAdministradoresConversacion()
    {
      try
      {
        int userId = getUser();

        var conductorInfo = await conductorService.getInfoID(userId);


        var resultConversacion = await conductorService.ObtineAdministradoresPorEmpresa((int)conductorInfo.EmpresaIdempresa!);

        return _suVanResponseService.Handle(resultConversacion);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }

    }


    [HttpPost]
    [Route("CerrarConversacion")]
    [SwaggerOperation(Description = "Cerrar conversacion con el administrador")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<int>))]
    [AllowAnonymous]
    public async Task<ActionResult> CerrarConversacion([FromBody] CerrarConversacionModel model)
    {
      try
      {
        var resultConversacion = await _conversacionesService.ModificarEstatus(model.ConversacionId, 0);

        return _suVanResponseService.Handle(resultConversacion);

      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }

    }


    [HttpPost]
    [Route("CreaConversacion")]
    [SwaggerOperation(Description = "Enviar mensaje al administrador")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<MensajeConversacion>))]

    public async Task<ActionResult> CreaConversacion([FromBody] CrearConversacion model)
    {
      try
      {
        SuVanResponse<MensajeConversacion> response = new();
        int userId = getUser();

        var conductorInfo = await conductorService.getInfoID(userId);

        MensajeConversacion mensajeConversacion = new()
        {
          EmpresaId = (int)conductorInfo.EmpresaIdempresa!,
          RutaId = null,
          OperadorId = userId,
          NombreConversacion = conductorInfo.Nombre!,
          UsuarioIdCreacion = model.UsuarioAdminId,
          //Mensaje = model.Mensaje!,
          TipoConversacion = 2
        };

        var conversaciones = await _conversacionesService.ObtenerConversacion(mensajeConversacion);

        if (conversaciones.Data.ConversacionId is not null)
        {
          response.CodigoMensaje = "402";
          response.Mensaje = "Existe una conversación en curso";
          return _suVanResponseService.Handle(response);
        }

        await NewConversation(mensajeConversacion);

        response.Data = mensajeConversacion;
        response.CodigoMensaje = "200";
        response.Mensaje = "Conversacion creada correctamente";


        return _suVanResponseService.Handle(response);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    private async Task NewConversation(MensajeConversacion model)
    {
      var connectionId = Guid.NewGuid().ToString();
      model.ConexionId = connectionId;


      //string _conexionId = model.ConexionId;
      if (!model.ConversacionId.HasValue)
      {
        var resultConversacion = await _conversacionesService.NuevaConversacion(new ConversacionModel()
        {
          TipoConversacion = model.TipoConversacion,
          EmpresaId = model.EmpresaId,
          RutaId = model.RutaId,
          OperadorId = model.OperadorId,
          NombreConversacion = model.NombreConversacion,
          UsuarioIdCreacion = model.UsuarioIdCreacion ?? default
        });

        if (resultConversacion.CodigoMensaje == "200")
        {
          model.ConversacionId = resultConversacion.Data.ConversacionId;
          //await _conversacionesService.RegistraMensaje(model.ConversacionId!.Value, model.OperadorId!.Value, 0, string.Empty);
          await _conversacionesService.RegistraActualizaConexion(model.ConversacionId!.Value, model.ConexionId);
          var listaConversaciones = await _conversacionesService.ConsultaBandeja(model.UsuarioIdCreacion!.Value, null);

          await _messageHub.Clients.Client(model.ConexionId).MessageSentToOperator(model);
          await _messageHub.Clients.Client(model.ConexionId).ConversationCreated(listaConversaciones.First(x => x.ConversacionId == model.ConversacionId.Value));
          await _messageHub.Clients.Client(model.ConexionId).RenderIbox(listaConversaciones);
        }


      }
      //return 0;
      //else
      //{
      //  await _conversacionesService.RegistraMensaje(model.ConversacionId.Value, model.UsuarioIdCreacion.Value, 1, model.Mensaje);
      //  await _conversacionesService.RegistraActualizaConexion(model.ConversacionId.Value, model.ConexionId);
      //  var listaConversaciones = await _conversacionesService.ConsultaBandeja(model.UsuarioIdCreacion.Value, null);
      //  await _messageHub.Clients.Client(model.ConexionId).MessageSentToOperator(model);
      //  await _messageHub.Clients.Client(model.ConexionId).RenderIbox(listaConversaciones);
      //}


    }

    [HttpPost]
    [Route("SendMessage")]
    [SwaggerOperation(Description = "Enviar mensaje al administrador mendiante una conversacion")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<MensajeToAdminResponse>))]

    public async Task<ActionResult> SendMessage([FromBody] MensajeOperadorModel model)
    {
      try
      {
        var conversacion = await _conversacionesService.ConsultaIdConexion(model.conversacion_id);
        var data = new
        {
          comentario = model.Mensaje,
          fechaCreacion = DateTime.Now
        };
        if (conversacion.CodigoMensaje == "200")
        {
          await _conversacionesService.RegistraMensaje(conversacion.Data.ConversacionId, model.conductor_id, 0, model.Mensaje);
          await _messageHub.Clients.Client(conversacion.Data.ConexionId).ReceiveMessage(data);
        }
        return _suVanResponseService.Handle(conversacion);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }

    [HttpGet]
    [Route("Conversaciones/{usuario_id}")]
    [SwaggerOperation(Description = "Consulta las conversaciones del operador")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<List<ConversacionModel>>))]
    public async Task<ActionResult> ConversacionesOperador(int usuario_id)
    {
      try
      {
        var conversaciones = await _conversacionesService.ConsultaConversacionesUsuario(usuario_id);
        conversaciones.CodigoMensaje = "200";
        conversaciones.Mensaje = string.Empty;
        return _suVanResponseService.Handle(conversaciones);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
    }
    [HttpGet]
    [Route("ConsultaMensajes/{conversacion_id}")]
    [SwaggerOperation(Description = "Consulta los mensajes por conversacion del operador")]
    [SwaggerResponse(200, Type = typeof(SuVanResponse<LoginResponse>))]
    public async Task<ActionResult> ConsultaMensajes(int conversacion_id)
    {
      try
      {
        var conversacion = await _conversacionesService.ObtenerConversacion(new MensajeConversacion
        {
          ConversacionId = conversacion_id
        });
        conversacion.CodigoMensaje = "200";
        conversacion.Mensaje = string.Empty;
        return _suVanResponseService.Handle(conversacion);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
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
