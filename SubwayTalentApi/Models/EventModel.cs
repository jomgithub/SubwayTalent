using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class EventModel
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
        public List<Genre> PreferredGenres { get; set; }
        public List<Skills> PreferredSkills { get; set; }
        public List<UserModel> Talents { get; set; }
        public List<UserModel> Planners { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public EventTypeModel Type { get; set; }
        public double Distance { get; set; }
        public string DeleteReason { get; set; }
    }
}