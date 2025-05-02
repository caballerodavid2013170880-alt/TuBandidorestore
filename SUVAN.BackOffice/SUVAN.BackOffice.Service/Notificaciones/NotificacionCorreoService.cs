using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SUVAN.BackOffice.Models.AppSettingsModels;
using SUVAN.BackOffice.Utilities;
using SUVAN.BackOffice.Utilities.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static SUVAN.BackOffice.Models.PayPal.Pago.OrdersCaptureExcepcionResponse;

namespace SUVAN.BackOffice.Service.Notificaciones
{
  public class NotificacionCorreoService : INotificacionCorreoService
  {
    private readonly MailSettingsOptions _mailOptions;

    private SmtpClient SmtpClient { get; }

    public NotificacionCorreoService(IOptions<MailSettingsOptions> mailOptions)
    {
      this._mailOptions = mailOptions.Value;

      SmtpClient = new SmtpClient()
      {
        Host = _mailOptions.Host,
        Port = _mailOptions.Port,
        Credentials = new NetworkCredential
        {
          UserName = _mailOptions.Username,
          Password = _mailOptions.Password
        }
      };

    }


    /// <summary>
    /// Envia codigo de descuento
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="codigo"></param>
    /// <param name="descuento"></param>
    /// <returns></returns>
    public async Task<bool> EnviarCodigoDescuentoPortal(string correo, string codigo, string descuento, DateTime hasta)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado/a </p>\n\n<p>Para expresar nuestro agradecimiento, nos complace ofrecerte un código de descuento exclusivo que puedes utilizar en tu próximo viaje. Este código te brinda la oportunidad de disfrutar de un descuento especial</p>\n\n<p>Código:  <b style='font-size:16pt'>{{Codigo}}</b></p>\n\n<p><b style='font-size:16pt'>{{Descuento}}</b></p>\n\n<p> ¡Aprovecha este beneficio! Este código es válido hasta el <strong>{{Vigencia}}</strong>, así que asegúrate de utilizarlo antes de esa fecha para no perder esta increíble oportunidad. </p>\n\n<p>Recibe un cordial saludo.</p>\n\n";


      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Descuento}}",descuento },
                                { "{{Codigo}}", codigo},
                                { "{{Vigencia}}", hasta.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","¡Código de descuento exclusivo!" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                content,
                lstCorreos,
                null!,
                _mailOptions.Username,
                "¡Tu código de descuento exclusivo ha llegado! SUVAN",
                SmtpClient
                );
      });

      /// el send email regresa 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }



    /// <summary>
    /// Correo para notificar al usuario sobre el codigo de verificacion para hacer login
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="codigo"></param>
    /// <returns></returns>
    public async Task<bool> EnviarCodigoPortal(string correo, string usuario, string codigo)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Hemos recibido una solicitud de inicio de sesi&oacute;n en el sistema. Para completar el proceso de inicio de sesi&oacute;n, utilice el siguiente c&oacute;digo de verificaci&oacute;n MFA:</p>\n\n<p>C&oacute;digo MFA:  <b style='font-size:16pt'>{{Codigo}}</b></p>\n\n<p> Este c&oacute;digo es válido por 10 minutos. No comparta este c&oacute;digo con nadie y úselo antes de que expire. </p>\n\n<p>Recibe un cordial saludo.</p>\n\n";


      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Codigo}}", codigo}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","C&oacute;digo de Verificaci&oacute;n MFA" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                content,
                lstCorreos,
                null!,
                _mailOptions.Username,
                "Código de Verificación Portal Administrativo SUVAN",
                SmtpClient
                );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> OlvidoPasswordApp(string correo, string usuario, string token)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Recibimos tu solicitud para reestablecer tu contrase&ntilde;a. El siguiente enlace ser&aacute; v&aacute;lido durante las pr&oacute;ximas 24 horas. Una vez que hayas realizado la recuperaci&oacute;n el enlace quedar&aacute; inactivo.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p> {{Url}} </p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", _mailOptions.RutaPortal, "Seguridad/VerificarOlvidoApp", token), "style='position: relative; left: 50%; transform: translateX(-50%);background-color:#004AAD; border-radius:6px; display:inline-block; margin-top:27px; padding:11px 19px; color: #FFFFFF; font-size: 14px; font-weight:500;font-family:Arial,Helvetica,sans-serif;' > Actualizar contrase&ntilde;a. </a>");

      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Url}}", linkurl}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Recuperar contraseña" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
          content,
          lstCorreos,
          null!,
          _mailOptions.Username,
          "Actualizar contraseña SUVAN",
          SmtpClient
          );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> OlvidoPassword(string correo, string usuario, string token)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Recibimos tu solicitud para reestablecer tu contrase&ntilde;a. El siguiente enlace ser&aacute; v&aacute;lido durante las pr&oacute;ximas 24 horas. Una vez que hayas realizado la recuperaci&oacute;n el enlace quedar&aacute; inactivo.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p> {{Url}} </p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", _mailOptions.RutaPortal, "Seguridad/VerificarOlvido", token), "style='position: relative; left: 50%; transform: translateX(-50%);background-color:#004AAD; border-radius:6px; display:inline-block; margin-top:27px; padding:11px 19px; color: #FFFFFF; font-size: 14px; font-weight:500;font-family:Arial,Helvetica,sans-serif;' > Actualizar contrase&ntilde;a. </a>");

      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Url}}", linkurl}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Recuperar contraseña" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                content,
                lstCorreos,
                null!,
                _mailOptions.Username,
                "Actualizar contraseña Portal Administrativo SUVAN",
                SmtpClient
                );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> EnviarCorreoActivacionPortal(string correo, string usuario, string token)
    {
      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p>Estimado/a <strong>{{Usuario}},</strong></p>\n\n<p>¡Te damos la bienvenida! Nos complace informarte que tu cuenta ha sido creada por un administrador y ahora estás listo/a para comenzar.</p>\n\n<p>Para garantizar la seguridad de tu cuenta, te pedimos que tomes un momento para actualizar tu contrase&ntilde;a. Puedes hacerlo fácilmente haciendo clic en el siguiente enlace:</p>\n\n<p> {{Url}} </p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", _mailOptions.RutaPortal, "Seguridad/Verificar", token), "style='position: relative; left: 50%; transform: translateX(-50%);background-color:#004AAD; border-radius:6px; display:inline-block; margin-top:27px; padding:11px 19px; color: #FFFFFF; font-size: 14px; font-weight:500;font-family:Arial,Helvetica,sans-serif;' > Actualizar contrase&ntilde;a. </a>");
      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Usuario}}",usuario },
                                { "{{Url}}", linkurl}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}"," ¡Bienvenido al Portal Administrativo SUVAN!" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                content,
                lstCorreos,
                null!,
                _mailOptions.Username,
                "¡Bienvenido al Portal Administrativo SUVAN!",
                SmtpClient
                );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <param name="ruta"></param>
    /// <returns></returns>
    public async Task<bool> ActivacionCuenta(string correo, string usuario, string codigo, string token, string ruta)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Esté estu código <b style='font-size:16pt'>{{Codigo}}</b> para activar tu cuenta.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      //string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", ConfiguracionCorreos.ServidorLocal.ToString(), "Cuenta/Verificando", token), "style = 'color:#00a1de;text-decoration:none;' > Para recuperar tu contrase&ntilde;a da clic. </a>");
      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Codigo}}", codigo}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Activación de cuenta" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                      content,
                      lstCorreos,
                      null!,
                      _mailOptions.Username,
                      "Activación de cuenta",
                      SmtpClient
                      );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <param name="ruta"></param>
    /// <returns></returns>
    public async Task<bool> EnviaCodigo(string correo, string usuario, string codigo, string token, string ruta)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Esté estu código <b style='font-size:16pt'>{{Codigo}}</b>.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      //string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", ConfiguracionCorreos.ServidorLocal.ToString(), "Cuenta/Verificando", token), "style = 'color:#00a1de;text-decoration:none;' > Para recuperar tu contrase&ntilde;a da clic. </a>");
      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Codigo}}", codigo}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Código de cuenta" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                      content,
                      lstCorreos,
                      null!,
                      _mailOptions.Username,
                      "Código de cuenta",
                      SmtpClient
                      );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <param name="ruta"></param>
    /// <returns></returns>
    public async Task<bool> InstruccionesRecuperaPassword(string correo, string usuario, string codigo, string token, string ruta)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Recibimos tu solicitud para reestablecer tu contrase&ntilde;a. El siguiente enlace ser&aacute; v&aacute;lido durante las pr&oacute;ximas 24 horas. Una vez que hayas realizado la recuperaci&oacute;n el enlace quedar&aacute; inactivo.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p> {{Url}} </p>\n\n<p>Recibe un cordial saludo.</p>\n\n";
      token = TokenCorreoPortal.GeneraToken(correo, 1);// se da una vigencia de 1 dias al token en el alta

      string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", _mailOptions.RutaPortal, "Seguridad/VerificarOlvidoAppCond", token), "style='position: relative; left: 50%; transform: translateX(-50%);background-color:#004AAD; border-radius:6px; display:inline-block; margin-top:27px; padding:11px 19px; color: #FFFFFF; font-size: 14px; font-weight:500;font-family:Arial,Helvetica,sans-serif;' > Actualizar contrase&ntilde;a. </a>");

      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Url}}",linkurl}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Recuperar contraseña" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                content,
                lstCorreos,
                null!,
                _mailOptions.Username,
                "Instrucciones para actualizar tu contraseña",
                SmtpClient
                );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <param name="ruta"></param>
    /// <returns></returns>
    public async Task<bool> SolicitudActualizaTelefono(string correo, string usuario, string codigo, string token, string ruta)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Esté estu código <b style='font-size:16pt'>{{Codigo}}</b>.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      //string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", ConfiguracionCorreos.ServidorLocal.ToString(), "Cuenta/Verificando", token), "style = 'color:#00a1de;text-decoration:none;' > Para recuperar tu contrase&ntilde;a da clic. </a>");
      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Codigo}}", codigo}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Solicitud actualización de teléfono" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                      content,
                      lstCorreos,
                      null!,
                      _mailOptions.Username,
                      "Solicitud actualización de teléfono",
                      SmtpClient
                      );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }




    public async Task<bool> EnviarCodigoPagoMonedero(string correo, string usuario, string codigo, string token, string ruta)
    {
      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p><strong>{{Fecha}}</strong></p>\n\n<p>Estimado <strong>{{Usuario}},</strong></p>\n\n<p>Esté estu código <b style='font-size:16pt'>{{Codigo}}</b>.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      //string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", ConfiguracionCorreos.ServidorLocal.ToString(), "Cuenta/Verificando", token), "style = 'color:#00a1de;text-decoration:none;' > Para recuperar tu contrase&ntilde;a da clic. </a>");
      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Codigo}}", codigo}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Código de cuenta" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                      content,
                      lstCorreos,
                      null!,
                      _mailOptions.Username,
                      "Código para confirmar pago",
                      SmtpClient
                      );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="correo"></param>
    /// <param name="usuario"></param>
    /// <param name="token"></param>
    /// <param name="ruta"></param>
    /// <param name="Attachments">Se recibe diccionario de bytes de archivos a adjuntar</param>
    /// <returns></returns>
    public async Task<bool> EnvioFactura(string correo, string usuario, string codigo, string token, string ruta, Dictionary<string, byte[]> Attachments = null)
    {

      string directorioBase = AppDomain.CurrentDomain.BaseDirectory;
      string urlBaseTemplate = $@"{directorioBase}Plantilla\CorreoBase.html";
      int resultadoEnvioCorreo = 0;
      Email Email = new Email();
      var prefile = string.Empty;

      var lstCorreos = new List<string>();
      lstCorreos.Add(correo);

      var body = "<p>Te compartimos tu factura electrónica.</p>\n\n<p>En caso que tu no hayas realizado la solicitud, favor de ignorar este mensaje.</p>\n\n<p>Recibe un cordial saludo.</p>\n\n";

      //string linkurl = string.Format("{0}{1}{2}", "<a href='", string.Format("{0}/{1}?value={2}'", ConfiguracionCorreos.ServidorLocal.ToString(), "Cuenta/Verificando", token), "style = 'color:#00a1de;text-decoration:none;' > Para recuperar tu contrase&ntilde;a da clic. </a>");
      var tokens = new Dictionary<string, string>()
                                {
                                { "{{Fecha}}", DateTime.Now.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-MX"))},
                                { "{{Usuario}}",usuario },
                                { "{{Codigo}}", codigo}
                                };

      prefile = MailService.BuildHtml(new Dictionary<string, string>()
            {
                { "{{body}}",body },
                { "{{header}}","Documento Electrónico" }
            }, MailService.ReadFile(urlBaseTemplate));

      var content = MailService.BuildHtml(tokens, prefile);

      await Task.Run(() =>
      {
        return resultadoEnvioCorreo = Email.SendMail(
                      content,
                      lstCorreos,
                      null!,
                      _mailOptions.Username,
                      "Documento Electrónico",
                      SmtpClient,
                      Attachments
                      );
      });

      /// el send mail regrea 1 si esta correcto 0 si fallo
      return resultadoEnvioCorreo == 1 ? true : false;
    }

  }
}
