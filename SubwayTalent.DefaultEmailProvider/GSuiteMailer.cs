using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.DefaultEmailProvider
{
    public class GSuiteMailer : IEmailProvider, IDisposable
    {
        private MailMessage _message = null;
        private SmtpClient _smtpClient = null;

        public string Body { get; set; }

        public IList<string> Cc { get; set; }

        public MailAddress From { get; set; }

        public string HTMLBody { get; set; }

        public string Password { get; set; }

        public string SMTPServer { get; set; }

        public string Subject { get; set; }

        public IList<string> To { get; set; }

        public string Username { get; set; }

        public string Domain { get; set; }

        public GSuiteMailer()
        {
            this._smtpClient = new SmtpClient();
        }

        public void Send()
        {
            this._message = new MailMessage();
            
            this._smtpClient.Host = this.SMTPServer;
            this._message.From = this.From;
            this.To.ToList().ForEach((string address) =>
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    this._message.To.Add(new MailAddress(address));
                }

            });

            this.Cc.ToList().ForEach((string address) =>
            {
                if (!string.IsNullOrWhiteSpace(address))
                {
                    this._message.CC.Add(new MailAddress(address));
                }

            });

            this._message.Subject = this.Subject;
            this._message.IsBodyHtml = !string.IsNullOrEmpty(this.HTMLBody);
            this._message.Body = !string.IsNullOrEmpty(this.HTMLBody) ? this.HTMLBody : this.Body;

            this._smtpClient.EnableSsl = true;
            this._smtpClient.UseDefaultCredentials = false;
            this._smtpClient.Credentials = new NetworkCredential(this.Username, this.Password,this.Domain);
            
            this._smtpClient.Port = 587;
            
            this._smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            this._smtpClient.Send(this._message);

            this._message = null;
        }

        public void Dispose()
        {
            this._message = null;
            this._smtpClient = null;
        }
        private bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;

        }
    }
}
