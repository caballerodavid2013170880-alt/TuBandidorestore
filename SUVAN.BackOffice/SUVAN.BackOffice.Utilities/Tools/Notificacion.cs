using RestSharp;
using System.Text.Json;

namespace SUVAN.BackOffice.Utilities.Tools
{
    public class Notificacion
    {
        /// <summary>
        /// Metodo para enviar una notificacion
        /// </summary>
        /// <param name="pFirebaseID">FireBaseID del usuario al que se enviara la notificación</param>
        /// <param name="pData">Objeto Data para la notificacion a enviar</param>
        /// <param name="pNotificacion">Objeto Notificacion a enviar</param>
        /// <returns> regresa 1 si la notificacion a sido enviada, 0 si no se pudo enviar</returns>
        public static int SendNotificacion(string pFirebaseID, object pData, string pTitle, string pMessage, string pClickAction, string pIcon, string pSound, string authFCM)
        {
            Models.Notification.Notificacion notification = new()
            {
                to = pFirebaseID ?? "",
                notification = new Models.Notification.Notification {
                    title = pTitle,
                    body = pMessage,
                    icon = pIcon,
                    sound = pSound,
                    click_action = pClickAction
                },
                data = pData
            };

            try
            {
                var client = new RestClient("https://fcm.googleapis.com/");
                var req = new RestRequest("fcm/send", Method.Post);
                req.AddHeader("Authorization", "Bearer " + authFCM);
                req.RequestFormat = DataFormat.Json;

                var jsonString = JsonSerializer.Serialize(notification);
                req.AddJsonBody(jsonString);
                var resp = client.ExecutePost(req);

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
