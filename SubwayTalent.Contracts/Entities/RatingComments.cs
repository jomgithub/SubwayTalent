using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts
{
    public class RatingComments
    {
        public string Comment { get; set; }
        public string Rating { get; set; }
        public string DateRated { get; set; }
        public string RatedBy { get; set; }
        public string Eventname { get; set; }
        public string RatedByName { get; set; }
        public string ProfilePic { get; set; }
    }
}
