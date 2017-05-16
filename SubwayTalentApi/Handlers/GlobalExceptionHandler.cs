using Newtonsoft.Json;
using SubwayTalent.Contracts.Entities;
using SubwayTalent.Core.Exceptions;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace SubwayTalentApi.Handlers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var requestMethod = context.ExceptionContext.Request.RequestUri.AbsolutePath;
            var requestBody = string.Empty;
            using (var stream = new MemoryStream())
            {
                var contextContent = (HttpContextBase)context.Request.Properties["MS_HttpContext"];
                contextContent.Request.InputStream.Seek(0, SeekOrigin.Begin);
                contextContent.Request.InputStream.CopyTo(stream);
                requestBody = Encoding.UTF8.GetString(stream.ToArray());
            }

            var exceptionMessage = (context.Exception.InnerException == null) ? context.Exception.Message : context.Exception.Message + ". " + context.Exception.InnerException.Message;
            exceptionMessage += ". " + context.Exception.StackTrace;

            SubwayContext.Current.Logger.Log(string.Format("[Exception] Method : {0}{1} Body:{2}{3} ExceptionInfo: {4}", requestMethod, Environment.NewLine + Environment.NewLine,
                                                             requestBody, Environment.NewLine + Environment.NewLine, exceptionMessage));


            if (context.Exception is SubwayTalentException)
            {
                context.Result = new OkNegotiatedContentResult<ResponseModel>(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = context.Exception.Message

                }, context.RequestContext.Configuration.Services.GetContentNegotiator(), context.Request, context.RequestContext.Configuration.Formatters);

            }
            else //general exception
            {
               
                ApplySpecialCases(requestMethod, requestBody, context);

                context.Result = new OkNegotiatedContentResult<ResponseModel>(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = "Failed. Please contact administrator."

                }, context.RequestContext.Configuration.Services.GetContentNegotiator(), context.Request, context.RequestContext.Configuration.Formatters);


            }
        }

        private void ApplySpecialCases(string requestMethod, string requestBody,ExceptionHandlerContext context)
        {

            if (requestMethod.Contains("AddPaymentMethod"))
            {
                var paymentDetails = JsonConvert.DeserializeObject<PaymentModel>(requestBody);

                SubwayContext.Current.PaymentRepo.AddPaymentException(new PaymentException
                {
                    UserId = paymentDetails.UserId,
                    StackTrace = context.Exception.StackTrace + ". " + ((context.Exception.InnerException != null) ? context.Exception.InnerException.StackTrace : string.Empty),
                    PaymentEventName = PaymentEvent.AddPaymentMethod,
                    Message = context.Exception.Message,
                    OtherInfo = JsonConvert.SerializeObject(paymentDetails)
                });
            }
        }
    }
}