using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class DropTalentModel
    {
        public int EventId { get; set; }

        public string UserId { get; set; }

        public bool Performed { get; set; }

        public string Comments { get; set; }

        public string UpdatedBy { get; set; }
    }
}