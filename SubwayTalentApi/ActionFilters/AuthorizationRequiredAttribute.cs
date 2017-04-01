using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SubwayTalentApi.Security;
using System;
using System.Configuration;

namespace SubwayTalentApi.ActionFilters
{
   // [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var authOn =  Convert.ToBoolean(ConfigurationManager.AppSettings["AuthorizationOn"]);
            
            if (authOn)
            {
                //  Get API key provider
                var provider = new TokenServices();

                //var provider = filterContext.ControllerContext.Configuration
                //    .DependencyResolver.GetService(typeof(ITokenServices)) as ITokenServices;

                if (filterContext.Request.Headers.Contains(Token))
                {
                    var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

                    // Validate Token
                    if (provider != null && !provider.ValidateToken(tokenValue))
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request" };
                        filterContext.Response = responseMessage;
                    }
                }
                else
                {
                    filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }

            base.OnActionExecuting(filterContext);

        }
    }
}