using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Interfaces
{
    public interface IEmailProvider : IDisposable
    {
        string Body { get; set; }

        IList<string> Cc { get; set; }

        MailAddress From { get; set; }

        string HTMLBody { get; set; }

        string Password { get; set; }

        string SMTPServer { get; set; }

        string Domain { get; set; }

        string Subject { get; set; }

        IList<string> To { get; set; }

        string Username { get; set; }

        void Send();
    }
}
