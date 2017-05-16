using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public static class EmailSender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="to"></param>
        /// <param name="body"></param>
        /// <param name="subject"></param>
        public static void SendMail(string cc, string to, string body, string subject)
        {
            try
            {

                var provider = SubwayContext.Current.EmailProvider;
                provider.To = new List<string>();
                provider.Body = string.Empty;
                provider.Subject = string.Empty;

                provider.From = new MailAddress(ConfigurationManager.AppSettings["From.Address"], ConfigurationManager.AppSettings["From.DisplayName"]);
                provider.Cc = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MailCC"])) ? new List<string>() : ConfigurationManager.AppSettings["MailCC"].Split(new char[] { ',' }).ToList();

                if (!string.IsNullOrWhiteSpace(to))
                    provider.To = to.Split(new char[] { ',' }).ToList();


                provider.HTMLBody = body;
                //provider.Body = body;
                provider.Subject = subject;
                provider.SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
                provider.Send();

            }
            catch (Exception ex)
            {
                var exceptionMessage = "Send email failed. " + ((ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message);
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
            }

        }
    }
}