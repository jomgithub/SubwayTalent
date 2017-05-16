using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Utilities
{
    public class SubwayMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public SubwayMultipartFormDataStreamProvider(string path)
            : base(path) { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            //Check for valid files in the appSettings.
            //if (!(InvalidImage(headers) || InvalidVideo(headers)))
            if (!(InvalidImage(headers) || InvalidVideo(headers)) || (InvalidImage(headers) && InvalidVideo(headers)))
                throw new Exception("Invalid file. Only JPEG and MP4 are accepted.");

            var fileNameGuid = Guid.NewGuid();
            var fileExtension = System.IO.Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));

            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? fileNameGuid + fileExtension : "NoName";
            //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
            return name.Replace("\"", string.Empty);
        }

        private bool InvalidImage(System.Net.Http.Headers.HttpContentHeaders headers)
        {          
            return (ConfigurationManager.AppSettings["ImageMimeTypes"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().Trim() == headers.ContentType.MediaType.ToLower().Trim()) == null);
        }
        private bool InvalidVideo(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            return (ConfigurationManager.AppSettings["VideoMimeTypes"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().Trim() == headers.ContentType.MediaType.ToLower().Trim()) == null);
        }
    }
}
