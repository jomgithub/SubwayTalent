using AttributeRouting.Web.Http;
using SubwayTalent.Core;
using SubwayTalent.Core.Utilities;
using SubwayTalent.Logging;
using SubwayTalentApi.ActionFilters;
using SubwayTalentApi.Facebook;
using SubwayTalentApi.Filters;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SubwayTalentApi.Controllers
{

   
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        public IHttpActionResult GetCities(string id)
        {
            try
            {
                return Ok(id.ToString());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

      
        [HttpPost]
        public IHttpActionResult testPost2(testClass postValue)
        {
            //if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            //{
            //    var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
            //    if (basicAuthenticationIdentity != null)
            //    {
            //        var userId = basicAuthenticationIdentity.UserId;
            //        return GetAuthToken(userId);
            //    }
            //}

            return Ok(postValue);
        }

        // POST api/values
        //[ApiAuthenticationFilter]
        //[AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult testPost()
        {
            //if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            //{
            //    var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
            //    if (basicAuthenticationIdentity != null)
            //    {
            //        var userId = basicAuthenticationIdentity.UserId;
            //        return GetAuthToken(userId);
            //    }
            //}


           var accessToken =  SubwayContext.Current.PayPalProcessor.CreateAccessToken("R23AAGjCg-0o0pYOPIuvliwWBxvo5eM15TtaIrI96daK8wZn3gcAw6skjaU4uV4TetUqGjjixNK0XGNOfq_nFYjiWr5D33yiEy8utv-QXVDZg3oC4f20Znb8tRpkTQ8WZnoYrkA6LNTZXNJazqxFg");


            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = accessToken
            });
        }

        [HttpPost]
        public IHttpActionResult testNotif()
        {
            try
            {

                var payload = new SubwayNotificationPayload();
                payload.DeviceToken = "C9BAE4A4EDD39E151ADEC54FE771A212E592820F0A2D9B79E9E6C25FE88E3F4C";
                payload.Device = SubwayTalent.Core.DeviceType.Apple;
                payload.AlertMessage = string.Format("You've been {0} by {1}", "Rejected", "David Foster");
                payload.Badge = 1;
                payload.Device = DeviceType.Apple;
                payload.AddCustom("status", "Rejected");
                payload.AddCustom("eventId", 23);
                payload.AddCustom("talentName", "Celine Dion");
                payload.AddCustom("plannerName", "David Foster");
                payload.AddCustom("eventName", "David and Friend's");
                payload.AddCustom("talentId", "celine01");
                payload.AddCustom("plannerId", "david@davidfoster.com");

                SubwayContext.Current.AppleNotification.Send(payload);

                return Ok("notif ok.");
            }
            catch (Exception ex)
            {

                var errorMsg = ex.Message + " " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty);
                FileLogger log = new FileLogger();
                log.Log(errorMsg);

                return Ok("Test Failed");
            }

        }
        [HttpPost]
        public IHttpActionResult testpayloadmultipart()
        {
            try
            {
                Stream str = new MemoryStream();
                string requestStream;
                using (var reader = new StreamReader(Request.Content.ReadAsStreamAsync().Result))
                {
                    requestStream = reader.ReadToEnd();
                    SubwayContext.Current.Logger.Log(requestStream);
                }

                return Ok("payload was logged.");
            }
            catch (Exception ex)
            {

                var errorMsg = ex.Message + " " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty);
                FileLogger log = new FileLogger();
                log.Log(errorMsg);

                return Ok("Test Failed");
            }

            //If Request.InputStream IsNot Nothing Then
            //              Using paramsReader As New IO.StreamReader(Request.InputStream)
            //                  _requestInputStream = paramsReader.ReadToEnd
            //                  Request.InputStream.Position = 0

            //                  'Try to read if there's any multipart contents in the request
            //                  ReadMultiPartRequest()
            //              End Using
            //          End If
        }



        [HttpPost]
        public Task<ResponseModel> testpayload()
        {
            var retVal = ProcessEventMultipart();
            return retVal;
        }

        private Task<ResponseModel> ProcessEventMultipart()
        {


            var userId = Request.Headers.GetValues("UserId").FirstOrDefault();

            var folderName = string.Format("{0}/{1}", ConfigurationManager.AppSettings["Uploads"], userId);
            var PATH = HttpContext.Current.Server.MapPath(string.Format("{0}{1}", "~/", folderName));
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
            if (!Directory.Exists(PATH))
                Directory.CreateDirectory(PATH);

            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new SubwayMultipartFormDataStreamProvider(PATH);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<ResponseModel>(t =>
                {

                    if (t.IsFaulted || t.IsCanceled)
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);

                    //TODO: separate the logic of getting files frrom getting data                   
                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        return new FileDesc(info.Name, rootUrl + "/" + folderName + "/" + info.Name, info.Length / 1024, i.Headers.ContentType.MediaType);
                    });





                    // var eventModel = ParseEventData(streamProvider);

                    //eventModel.Picture = fileInfo.ToList()[0].Path;

                    var response = new ResponseModel
                    {
                        Status = Status.Success,
                        Data = fileInfo
                    };

                    return response;
                });


                return task;
            }
            else
                throw new Exception("The request is not multi-part content.");
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        private EventModel ParseEventData(SubwayMultipartFormDataStreamProvider streamProvider)
        {
            var eventModel = new EventModel();
            eventModel.Planners = new List<UserModel>();
            eventModel.Talents = new List<UserModel>();

            PopulateEventModelProperties(streamProvider, eventModel);

            return eventModel;
        }

        private void PopulateEventModelProperties(SubwayMultipartFormDataStreamProvider streamProvider, EventModel eventModel)
        {
            foreach (var data in streamProvider.FormData)
            {
                PropertyInfo prop = eventModel.GetType().GetProperty(data.ToString());

                switch (data.ToString())
                {
                    case "Planners[][UserId]":
                        {
                            var planners = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var planner in planners)
                            {
                                eventModel.Planners.Add(new UserModel
                                {
                                    UserId = planner
                                });
                            }

                            break;
                        }
                    case "Talents[][UserId]":
                        {
                            var talents = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var talent in talents)
                            {
                                eventModel.Talents.Add(new UserModel
                                {
                                    UserId = talent
                                });
                            }
                            break;
                        }
                    case "Type[Id]":
                        {
                            eventModel.Type = new EventTypeModel
                            {
                                Id = ConvertToProperType("Type", streamProvider.FormData[data.ToString()])
                            };
                            break;
                        }
                    default:
                        {
                            prop.SetValue(eventModel, ConvertToProperType(prop.PropertyType.Name, streamProvider.FormData[data.ToString()]));
                            break;
                        }
                }
            }
        }

        private dynamic ConvertToProperType(string propertyType, string val)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Int32.Parse(val);
                case "DateTime":
                    return DateTime.Parse(val);
                default:
                    return val;
            }
        }

    }



    public class testClass
    {

        public string Name;

        public short LastName;
    }
}