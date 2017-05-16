using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class FileDesc
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string FileType { get; set; }
        public string ThumbNailPath { get; set; }

        public FileDesc(string name, string path, long size,string mimeType,string fileType, string thumbnail = null)
        {
            Name = name;
            Path = path;
            Size = size;
            MimeType = mimeType;
            FileType = fileType;
            ThumbNailPath = thumbnail;
        }
        public FileDesc(string name, string path, long size, string mimeType)
        {
            Name = name;
            Path = path;
            Size = size;
            MimeType = mimeType;
           
        }
    }
}