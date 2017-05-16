using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubwayTalent.Core.Interfaces
{
    public interface  IThumbnailGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoPath">Must Include the filename as well   i.e. file.ext</param>
        /// <param name="thumbnailPath">Must Include the filename as well   i.e. file.ext</param>
        /// <param name="framePosition">this is the frame position</param>
        void Generate(string videoPath, string thumbnailPath, float? framePosition = 5);
    }
}
