using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SubwayTalentApi.Handlers
{
    public class SubwayTalentMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var debugMode = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["DebugMode"]) ? false : true;

            if (debugMode)
            {
                var requestMethod = request.RequestUri.AbsolutePath;
                var requestBody = string.Empty;
                using (var stream = new MemoryStream())
                {
                    var contextContent = (HttpContextBase)request.Properties["MS_HttpContext"];
                    contextContent.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    contextContent.Request.InputStream.CopyTo(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                }
                SubwayContext.Current.Logger.Log(string.Format("Request Method : {0}{1} Body:{2}{3}", requestMethod, Environment.NewLine + Environment.NewLine,
                                                             requestBody, Environment.NewLine + Environment.NewLine));
            }

            return base.SendAsync(request, cancellationToken);
        }        
    }
}