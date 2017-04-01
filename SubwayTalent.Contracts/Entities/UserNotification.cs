using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public Int16 StatusId { get; set; }
        public string UpdatedBy { get; set; }
        public Event EventDetails { get; set; }
        public UserAccount UpdatedByInfo { get; set; }
        public UserAccount UserInfo { get; set; }
        //Status - 1, Eventupate - 2
        public NotificationType NotificationType { get; set; }
    }

    public enum NotificationType : byte
    {
        StatusChange = 1,
        EventUpdate = 2
    }
}
