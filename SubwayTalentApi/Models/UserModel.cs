using SubwayTalent.Contracts;
using SubwayTalent.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Models
{
    public class UserModel
    {               
        public string AccessToken;
        public string UserId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime LastLoggedInDate { get; set; }
        public DateTime LockedOutDate { get; set; }
        public Boolean FacebookUser { get; set; }
        public List<Skills> Skills { get; set; }
        public List<Genre> Genres { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public float Rate { get; set; }
        public float Rating { get; set; }
        public float RatingTalent { get; set; }

        /// <summary>
        /// For Planner
        /// </summary>
        public string ProfilePic { get; set; }       
        public string Gender { get; set; }
        public string MobileNumber { get; set; }

        public string TalentName { get; set; }
        public List<ExternalMedia> SoundCloud { get; set; }
        public List<ExternalMedia> Youtube { get; set; }
        public string Comments { get; set; }
        /// <summary>
        /// Status for the event. 
        /// 0 - pending/invited, 1 - accepted, 2 - rejected/declined, 3 - requested by talent, 4 - dropped
        /// </summary>
        public int Status { get; set; }
        public float EventRate { get; set; }

        public string ProfilePicTalent { get; set; }
        public string Perspective { get; set; }
        public string DeviceToken { get; set; }
        public Int16 Device { get; set; }
        public bool AllowEmail { get; set; }
        public bool AllowNotification { get; set; }
        //for sorting
        public double Distance { get; set; }
        public int CityStateId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string StateId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public List<Payment> PaymentMethod { get; set; }

        public short PaymentStatus { get; set; }
        public DateTime PaymentDateUpdate { get; set; }
        
    }

    public class LoginModel
    {
        public UserModel User { get; set; }
        public string Token { get; set; }
        public string TokenExpiry { get; set; }
    }
}