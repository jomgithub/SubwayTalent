using SubwayTalent.Core.Exceptions;
using SubwayTalent.Core.Utilities;
using SubwayTalentApi.ActionFilters;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SubwayTalentApi.Controllers
{
    
    public class MediaController : SubwayBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpPost]
        public Task<ResponseModel> UploadMedia()
        {

            var userIdheader = Request.Headers.FirstOrDefault(h =>
                 {
                     return h.Key == "UserId";
                 });

            if (userIdheader.Key == null)
            {

                var task = new Task<ResponseModel>(() =>
                   {
                       return new Models.ResponseModel
                       {
                           Status = Models.Status.Failed,
                           ErrorMessage = "Invalid header value. Should be UserId."
                       };
                   });
                task.RunSynchronously();

                return task;
            }


            var userId = Request.Headers.GetValues("UserId").FirstOrDefault();
            var folderName = string.Format("{0}/{1}", ConfigurationManager.AppSettings["Uploads"], userId);
            var PATH = HttpContext.Current.Server.MapPath(string.Format("{0}{1}", "~/", folderName));
            var environment = ConfigurationManager.AppSettings["Environment"];
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
            if (!Directory.Exists(PATH))
                Directory.CreateDirectory(PATH);

            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new SubwayMultipartFormDataStreamProvider(PATH);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<ResponseModel>(t =>
                {
                    try
                    {
                        //Todo: put logs here.
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);


                        var fileInfo = streamProvider.FileData.Select(i =>
                        {
                            var fileType = string.Empty;
                            var thumbnailPhysicalPath = string.Empty;
                            var info = new FileInfo(i.LocalFileName);
                            var filePath = rootUrl + "/" + (string.IsNullOrWhiteSpace(environment) ? string.Empty : environment + "/")                                
                                            + folderName + "/" + info.Name;

                            if (SubwayContext.Current.VideMimeTypesList.FirstOrDefault(f => f.Trim().ToLower() == i.Headers.ContentType.MediaType) != null)
                            {
                                fileType = "V";
                                thumbnailPhysicalPath = string.Format("{0}/{1}.jpg", PATH, info.Name);
                                SubwayContext.Current.ThumbnailGenerator.Generate(filePath, thumbnailPhysicalPath, 10f);
                                return new FileDesc(info.Name, filePath, info.Length / 1024, i.Headers.ContentType.MediaType, fileType, filePath + ".jpg");

                            }

                            if (SubwayContext.Current.ImageMimeTypesList.FirstOrDefault(f => f.Trim().ToLower() == i.Headers.ContentType.MediaType) != null)
                                fileType = "P";


                            return new FileDesc(info.Name, filePath, info.Length / 1024, i.Headers.ContentType.MediaType, fileType,"");
                        });



                        foreach (var file in fileInfo)
                        {
                            SubwayContext.Current.UserRepo.AddFile(userId, file.FileType, file.Path, file.Name, file.ThumbNailPath);
                        }


                        return new ResponseModel
                        {
                            Status = Status.Success,
                            Data = fileInfo
                        };

                    }
                    catch (Exception ex)
                    {
                        var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                        exceptionMessage += ". " + ex.StackTrace;

                        SubwayContext.Current.Logger.Log(exceptionMessage);
                        return new ResponseModel
                        {
                            Status = Status.Failed,
                            ErrorMessage = exceptionMessage
                        };
                    }
                });
                return task;
            }
            else
                throw new SubwayTalentException("The request is not multi-part content.");
        }


    }
}
