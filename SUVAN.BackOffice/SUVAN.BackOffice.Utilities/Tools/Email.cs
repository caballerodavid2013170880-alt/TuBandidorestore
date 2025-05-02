using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Utilities.Tools
{
  public class Email
  {
    /// <summary>
    /// Metodo para formar el email y enviarlo
    /// </summary>
    /// <param name="content"></param>
    /// <param name="correos"></param>
    /// <param name="correoBcc"></param>
    /// <param name="fromEmail"></param>
    /// <param name="asunto"></param>
    /// <returns> regresa 1 si el email a sido enviado 0 si no se pudo enviar</returns>
    public static int SendMail(string content, List<string> correos, List<string> correoBcc, string fromEmail, string asunto, SmtpClient smtpClient, Dictionary<string, byte[]> Attachments = null)
        {
      Thread.CurrentThread.CurrentCulture = new CultureInfo("es-Es");
      MailService.Subject = asunto;
      MailService.SmtpClient = smtpClient;
      MailService.fromEmail = fromEmail;

      if (correos != null)
      {
        MailService.Send(correos.Distinct().ToList(), content, correoBcc, correoBcc, Attachments);
        return 1;
      }
      if (correos == null && correoBcc.Count > 0)
      {
        MailService.Send(content, correoBcc);
        return 1;
      }            
      else { return 0; }
    }
  }
}
