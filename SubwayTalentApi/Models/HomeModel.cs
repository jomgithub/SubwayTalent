using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class HomeModel
    {
        /// <summary>
        /// 1 true, 0 false
        /// </summary>
        public short RemoveExpiredDates { get; set; }
        public string UserId { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public string Timezone { get; set; }
    }
}