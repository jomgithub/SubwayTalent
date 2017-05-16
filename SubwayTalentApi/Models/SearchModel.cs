using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class SearchModel
    {
        public string UserId { get; set; }
        public string SearchString { get; set; }
        public List<int> GenreList { get; set; }
        public List<int> SkillList { get; set; }
        public double DistanceInMeters { get; set; }
        public Sort Sort { get; set; }
        public Location CurrentLocation { get; set; }
        public List<int> CityStateID { get; set; }
    }

    public class Sort
    {
        public string Key { get; set; }
        public string Direction { get; set; }
    }
 
    public class Location
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    public class SetProfilePicModel
    {
        public string UserId { get; set; }
        public int FileId { get; set; }
        public string Perspective { get; set; }
    }

    public class PasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string UserId { get; set; }
    }
}