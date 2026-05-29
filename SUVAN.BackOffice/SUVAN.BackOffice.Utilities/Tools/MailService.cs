using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SUVAN.BackOffice.Utilities.Tools
{
    public class MailService
    {
        public static string fromEmail { get; set; } = string.Empty;
        public static MailMessage messageFile { get; set; } = null!;
        public static SmtpClient SmtpClient { get; set; } = null!;
        public static string Subject { get; set; } = string.Empty;

        public static string BuildHtml(Dictionary<string, string> tokens, string html)
        {
            foreach (KeyValuePair<string, string> token in tokens)
                html = html.Replace(token.Key, token.Value);
            return html;
        }

        public static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="template"></param>
        /// <param name="CC"></param>
        /// <param name="BCC"></param>
        public static void Send(List<string> toEmail, string template, List<string> CC, List<string> BCC, Dictionary<string, byte[]> Attachments = null)
        {
            List<byte> files = new List<byte>();

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(fromEmail),
                Subject = Subject != null ? Subject : string.Empty,
                IsBodyHtml = true
            };
            messageFile = message;
            if (toEmail != null && toEmail.Any<string>())
            {
                foreach (string addresses in toEmail)
                    message.To.Add(addresses);
            }
            if (CC != null && CC.Any<string>())
            {
                foreach (string addresses in CC)
                    message.CC.Add(addresses);
            }
            if (BCC != null && BCC.Any<string>())
            {
                foreach (string addresses in BCC)
                    message.Bcc.Add(addresses);
            }
            SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpClient.EnableSsl = true;
            message.Body = template;

            if (Attachments != null)
            {
                foreach (KeyValuePair<string, byte[]> token in Attachments)
                {
                    message.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(token.Value), token.Key));
                }
            }

            SmtpClient.Send(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="template"></param>
        /// <param name="CC"></param>
        public static void Send(List<string> toEmail, string template, List<string> CC)
          => Send(toEmail, template, CC, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="BCC"></param>
        public static void Send(string template, List<string> BCC)
        => Send(null, template, null, BCC);



        public static byte[] ToEml(MailMessage message)
        {
            byte[] array;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                object obj = typeof(SmtpClient).Assembly.GetType("System.Net.Mail.MailWriter").GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder)null, new Type[1]
                {
          typeof (Stream)
                }, (ParameterModifier[])null).Invoke(new object[1]
                {
          (object) memoryStream
                });
                typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object)message, BindingFlags.Instance | BindingFlags.NonPublic, (Binder)null, new object[3]
                {
          obj,
          (object) true,
          (object) true
                }, (CultureInfo)null);
                obj.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, BindingFlags.Instance | BindingFlags.NonPublic, (Binder)null, new object[0], (CultureInfo)null);
                array = memoryStream.ToArray();
            }
            return array;
        }
    }
}
