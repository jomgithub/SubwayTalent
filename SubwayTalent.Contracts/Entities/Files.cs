using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubwayTalent.Contracts
{
    public class Files
    {
        public string UserId { get; set; }
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string FileType { get; set; }
        public string ThumbNailPath { get; set; }
    }
}
