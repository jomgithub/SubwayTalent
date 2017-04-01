using SendGrid;
using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.SendGridProvider
{
    public class SendGridEmailProvider : IEmailProvider
    {

        private SendGridMessage _message = null;

        private NetworkCredential _credentials = null;
        private Web _transport = null;

        public SendGridEmailProvider()
        {
            _message = new SendGridMessage();
        }

        public string Body { get; set; }

        public IList<string> Cc { get; set; }

        public MailAddress From { get; set; }

        public string HTMLBody { get; set; }

        public string Password { get; set; }

        public string SMTPServer { get; set; }

        public string Subject { get; set; }

        public IList<string> To { get; set; }

        public string Username { get; set; }

        public string Domain  { get; set; }

        public void Send()
        {
            this._message.From = this.From;
            this._message.To = this.To.Select((string address) => { return new MailAddress(address); }).ToArray();
            
            if (this.Cc != null)
                this._message.Cc = this.Cc.Select((string address) =>
                {
                    return new MailAddress(address);
                }).ToArray();


            this._message.Subject = this.Subject;
            this._message.Html = this.HTMLBody;
            this._message.Text = this.Body;
            if ((this._credentials == null))
            {
                this._credentials = new NetworkCredential(this.Username, this.Password);
            }
            if ((this._transport == null))
            {
                this._transport = new Web(this._credentials);
            }
            this._transport.Deliver(this._message);
        }

        public void Dispose()
        {
            this._message = null;
            this._transport = null;
            this._credentials = null;
        }
    }
}
