using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Notificaciones
{
    public interface INotificacionPushService
    {
        Task<bool> EnvioNotificacion(string pFirebaseID, object pData, string pTitle, string pMessage, string pClickAction, string pIcon = "suvan", string pSound = "defaultSoundUri");
    }
}
