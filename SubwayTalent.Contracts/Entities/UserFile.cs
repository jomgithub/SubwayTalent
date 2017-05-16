using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts
{
    public enum FileType
    { 
        Picture,
        Video
    }

    public class UserFile
    {
        public string Path{ get; set; }
        public FileType Type { get; set; }
    }
}
