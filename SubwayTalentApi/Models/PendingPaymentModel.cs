using SubwayTalent.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class PendingPaymentModel
    {
        public float Total{ get; set; }
        public List<Event> EventList { get; set; }
    }
}