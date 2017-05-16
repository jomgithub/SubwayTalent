using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubwayTalent.Contracts
{
    public class ExternalMedia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string  ThumbnailUrl { get; set; }
        public string ExternalType { get; set; }
    }
}
