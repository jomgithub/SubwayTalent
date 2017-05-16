using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public class ThumbnailGenerator : IThumbnailGenerator
    {
        public string VideoPath { get; set; }
        public string ThumnailPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoPath"></param>
        /// <param name="thumbnailPath"></param>
        /// <param name="framePosition"></param>
        public void Generate(string videoPath, string thumbnailPath, float? framePosition = 5)
        {
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            ffMpeg.GetVideoThumbnail(videoPath, thumbnailPath, framePosition);
        }

    }
}