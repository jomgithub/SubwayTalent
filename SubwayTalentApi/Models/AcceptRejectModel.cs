using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class AcceptRejectModel
    {
        public int EventId{ get; set; }
        public string UserId { get; set; }
        public short Status { get; set; }
        public string UpdatedBy { get; set; }
    }
}