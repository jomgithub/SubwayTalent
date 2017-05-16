using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubwayTalent.Contracts
{
    public class PlannerCounts
    {
        public int Invites { get; set; }
        public int Booked { get; set; }
        public int Closed { get; set; }    
    }

    public class TalentCounts
    {
        public int Leads { get; set; }
        public int Booked { get; set; }
        public int Closed { get; set; }
    }
}
