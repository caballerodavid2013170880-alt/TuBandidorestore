using SUVAN.BackOffice.Models.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.MensajeriaService
{
  public interface IHubSuVanService
  {
    public Task MessageSentToOperator(MensajeConversacion mensajeConversacion);
    Task SendMessageOperator(MensajeConversacion model);
    public Task ConversationCreated(BandejaConversacionesModel conversation);
    public Task RenderIbox(List<BandejaConversacionesModel> conversacionesModels);

    public Task ReceiveMessage(object message);
    Task<string> GetConnectionId(string user);
  }
}
