using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts.Interfaces
{
    public interface IEvent
    {
        void AddEvent(Event eventObj);
        /// <summary>
        /// get events for planner with conv
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Event> GetEventsPlanner(string userId);
        /// <summary>
        /// Gigs
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Event> GetEventsTalent(string userId);

        List<Event> GetAllEvents(string userId = null);

        List<Event> GetPlannerInvites(string userId);

        List<EventType> GetTalentInvites(string userId);
        void AddTalentToEvent(Event eventObj);
      
        void AddSkillsToEvent(Event eventObj, DbConnection conn, DbTransaction trans);
        void AddGenreToEvent(Event eventObj, DbConnection conn, DbTransaction trans);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userId"></param>
        /// <param name="status">0 pending, 1 accepted, 2 rejected, 3 requested</param>
        string AcceptRejectRequestTalentInvite(string userId, int eventId, int status,string updatedBy);

        List<string> RateTalentToEvent(Event eventObj);

        List<string> RatePlannerToEvent(Event eventObj);

        string Update(Event eventObj);

        Event GetEventDetails(int eventId, DbConnection connection = null);

        string DropTalent(int eventId, string talentId, string comments, bool performed, string updatedBy);

        List<Event> SearchEvent(string searchString, string genreList, string skillList, string userId = null);

        void DeleteEvent(int eventId);

        List<EventPlannerPayment> GetDoneEventsPaymentInfo();

        
    }
}
