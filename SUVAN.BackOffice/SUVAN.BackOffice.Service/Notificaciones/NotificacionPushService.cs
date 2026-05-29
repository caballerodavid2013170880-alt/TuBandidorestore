using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Service.Notificaciones
{
    public class NotificacionPushService : INotificacionPushService
    {
        private readonly IOptions<GlobalConfigsOptions> _GlobalConfigsOptions;

        public NotificacionPushService(IOptions<GlobalConfigsOptions> pGlobalConfigsOptions)
        {
            this._GlobalConfigsOptions = pGlobalConfigsOptions;
        }

        /// <summary>
        /// Envia codigo de descuento
        /// </summary>
        /// <param name="correo"></param>
        /// <param name="codigo"></param>
        /// <param name="descuento"></param>
        /// <returns></returns>
        public async Task<bool> EnvioNotificacion(string pFirebaseID, object pData, string pTitle, string pMessage, string pClickAction, string pIcon, string pSound)
        {
            int resultadoEnvioPush = 0;

            await Task.Run(() =>
            {
                return resultadoEnvioPush = Notificacion.SendNotificacion(
                    pFirebaseID,
                    pData,
                    pTitle,
                    pMessage,
                    pClickAction,
                    pIcon,
                    pSound,
                    _GlobalConfigsOptions.Value.authFCM
                    );
            });

            return resultadoEnvioPush == 1;
        }

    }
}
