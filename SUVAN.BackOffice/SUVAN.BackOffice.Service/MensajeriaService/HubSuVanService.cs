
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SUVAN.BackOffice.Database.Entities;
using SUVAN.BackOffice.Models.Mensajeria;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.MensajeriaService
{
  public class HubSuVanService : Hub<IHubSuVanService>
  {
    private readonly IConversacionesService _conversacionesService;

    public HubSuVanService(
            IConversacionesService conversacionesService)
    {
      _conversacionesService = conversacionesService;
    }
    public override Task OnConnectedAsync()
    {

      return base.OnConnectedAsync();
    }
    #region Envio y recepcion de mensajes Administrador - Operador

    [HubMethodName("SendMessageOperator")]
    public async Task SendMessageOperator(MensajeConversacion model)
    {
      string _conexionId = model.ConexionId;
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
          await _conversacionesService.RegistraMensaje(model.ConversacionId.Value, model.UsuarioIdCreacion.Value, 1, model.Mensaje);
          await _conversacionesService.RegistraActualizaConexion(model.ConversacionId.Value, _conexionId);
          var listaConversaciones = await _conversacionesService.ConsultaBandeja(model.UsuarioIdCreacion.Value, null);

          await Clients.Client(_conexionId).MessageSentToOperator(model);
          await Clients.Client(_conexionId).ConversationCreated(listaConversaciones.First(x => x.ConversacionId == model.ConversacionId.Value));
          await Clients.Client(_conexionId).RenderIbox(listaConversaciones);
        }
      }
      else
      {
        await _conversacionesService.RegistraMensaje(model.ConversacionId.Value, model.UsuarioIdCreacion.Value, 1, model.Mensaje);
        await _conversacionesService.RegistraActualizaConexion(model.ConversacionId.Value, _conexionId);
        var listaConversaciones = await _conversacionesService.ConsultaBandeja(model.UsuarioIdCreacion.Value, null);
        await Clients.Client(_conexionId).MessageSentToOperator(model);
        await Clients.Client(_conexionId).RenderIbox(listaConversaciones);
      }

    }

    [HubMethodName("GetListOfConversations")]
    public async Task GetListOfConversations(string conectionId, int userId)
    {
      var listaConversaciones = await _conversacionesService.ConsultaBandeja(userId, null);
      await Clients.Client(conectionId).RenderIbox(listaConversaciones);
    }

    [HubMethodName("updateConexionId")]
    public async Task updateConexionId(int conversacionId, string connectionId)
    {
      await _conversacionesService.RegistraActualizaConexion(conversacionId, connectionId);
    }

    #endregion
    public override Task OnDisconnectedAsync(Exception exception)
    {
      return base.OnDisconnectedAsync(exception);
    }



  }
}
