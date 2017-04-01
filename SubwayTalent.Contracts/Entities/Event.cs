using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateStart { get; set; }
        public string Description { get; set; }       
        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Picture { get; set; }
        public List<LookUpValues> PreferredGenres { get; set; }
        public List<LookUpValues> PreferredSkills { get; set; }
        public List<UserAccount> Talents { get; set; }
        public List<UserAccount> Planners { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public EventType Type { get; set; }
        public double Distance { get; set; }
        public string DeleteReason { get; set; }
    }
}
