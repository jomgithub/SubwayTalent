using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SubwayTalentApi.FakeObjects
{
    public class FakeEmailProvider : IEmailProvider
    {
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

        public void Send()
        {
            SubwayContext.Current.Logger.Log("<FakeEmail>");
            SubwayContext.Current.Logger.Log(string.Format("Body:{0}, From{1},HTMLBody:{2},Subject:{3}, To:{4}, Cc: {5}",
                                                            Body, From, HTMLBody, Subject, string.Join(" ", To.ToArray()),
                                                            string.Join(" ", Cc.ToArray())));
            SubwayContext.Current.Logger.Log("</FakeEmail>");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}