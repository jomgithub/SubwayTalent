using SubwayTalent.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public class EmailContentFactory
    {
        #region "Private Member(s)"

        private static EmailContent emailContent;

        private static string GetContent(string documentName, string plannerName, string talentName, string eventName, string eventdate)
        {
            emailContent = SubwayContext.Current.LookUpValuesRepo.GetContent(documentName);

            if (string.IsNullOrWhiteSpace(emailContent.Content))
                SubwayContext.Current.Logger.Log("Exception Email: " + documentName + " is missing.");


            return emailContent.Content.Replace("[planner]", plannerName)
                             .Replace("[talent]", talentName)
                             .Replace("[event]", eventName)
                             .Replace("[eventdate]", eventdate);

        }

        #endregion

        #region "Talent/Planner Event Email(s)"
        /// <summary>
        /// The Talent accepted to perform in the event
        /// </summary>
        /// <param name="plannerName"></param>
        /// <param name="talentName"></param>
        /// <param name="eventName"></param>
        /// <param name="eventdate"></param>
        /// <returns></returns>
        public static string GetAcceptedMessage(string plannerName, string talentName, string eventName, string eventdate)
        {
            return GetContent("EMAIL_TALENT_ACCEPTED", plannerName, talentName, eventName, eventdate);
        }

        /// <summary>
        /// Planner invited the talent to perform in the event.
        /// </summary>
        /// <param name="plannerName"></param>
        /// <param name="talentName"></param>
        /// <param name="eventName"></param>
        /// <param name="eventdate"></param>
        /// <returns></returns>
        public static string GetInviteMessage(string plannerName, string talentName, string eventName, string eventdate)
        {
            return GetContent("EMAIL_TALENT_INVITE", plannerName, talentName, eventName, eventdate);
        }

        /// <summary>
        /// The Planner accepted the request
        /// </summary>
        /// <param name="plannerName"></param>
        /// <param name="talentName"></param>
        /// <param name="eventName"></param>
        /// <param name="eventdate"></param>
        /// <returns></returns>
        public static string GetPlannerAcceptedMessage(string plannerName, string talentName, string eventName, string eventdate)
        {
            return GetContent("EMAIL_PLANNER_ACCEPTED", plannerName, talentName, eventName, eventdate);
        }

        /// <summary>
        /// The talent requested to perform in the event
        /// </summary>
        /// <param name="plannerName"></param>
        /// <param name="talentName"></param>
        /// <param name="eventName"></param>
        /// <param name="eventdate"></param>
        /// <returns></returns>
        public static string GetRequestedMessage(string plannerName, string talentName, string eventName, string eventdate)
        {
            return GetContent("EMAIL_REQUEST_TO_EVENT", plannerName, talentName, eventName, eventdate);
        }

        /// <summary>
        /// Talent rejected the invitation
        /// </summary>
        /// <param name="plannerName"></param>
        /// <param name="talentName"></param>
        /// <param name="eventName"></param>
        /// <param name="eventdate"></param>
        /// <returns></returns>
        public static string GetTalentRejectedMessage(string plannerName, string talentName, string eventName, string eventdate)
        {
            return GetContent("EMAIL_TALENT_REJECTED", plannerName, talentName, eventName, eventdate);
        }

        /// <summary>
        /// Planner rejected the talent request.
        /// </summary>
        /// <param name="plannerName"></param>
        /// <param name="talentName"></param>
        /// <param name="eventName"></param>
        /// <param name="eventdate"></param>
        /// <returns></returns>
        public static string GetPlannerRejectedMessage(string plannerName, string talentName, string eventName, string eventdate)
        {
            return GetContent("EMAIL_PLANNER_REJECTED", plannerName, talentName, eventName, eventdate);
        }

        #endregion

        public static string GetForgotPasswordTemplate(string userFullName, string tempPassword)
        {
            emailContent = SubwayContext.Current.LookUpValuesRepo.GetContent("EMAIL_FORGOT_PASSWORD");

            if (string.IsNullOrWhiteSpace(emailContent.Content))
                SubwayContext.Current.Logger.Log("Exception Email: EMAIL_FORGOT_PASSWORD is missing.");


            return emailContent.Content.Replace("[user]", userFullName)
                           .Replace("[password]", tempPassword);
        }

        public static string GetHelpTemplate(string userFullName)
        {
            emailContent = SubwayContext.Current.LookUpValuesRepo.GetContent("EMAIL_HELP");

            if (string.IsNullOrWhiteSpace(emailContent.Content))
                SubwayContext.Current.Logger.Log("Exception Email: EMAIL_HELP is missing.");


            return emailContent.Content.Replace("[Sender]", userFullName);

        }

        public static string GetHelpToAdminTemplate(string userFullName, string email, string message)
        {
            emailContent = SubwayContext.Current.LookUpValuesRepo.GetContent("EMAIL_HELP_TO_ADMIN");

            if (string.IsNullOrWhiteSpace(emailContent.Content))
                SubwayContext.Current.Logger.Log("Exception Email: EMAIL_HELP_TO_ADMIN is missing.");
            
            return emailContent.Content.Replace("[sender]", userFullName)
                                    .Replace("[email]", email)
                                    .Replace("[message]", message);           
        }

        public static string GetEventUpdateMsg(string recipient, string eventName)
        {
            emailContent = SubwayContext.Current.LookUpValuesRepo.GetContent("EMAIL_EVENT_UPDATE");

            if (string.IsNullOrWhiteSpace(emailContent.Content))
                SubwayContext.Current.Logger.Log("Exception Email: EMAIL_EVENT_UPDATE is missing.");

            return emailContent.Content.Replace("[recipient]", recipient)
                                    .Replace("[eventName]", eventName);

        }

        public static string GetDeleteEventMsg(string recipient, string eventName, string dateStart, string deleteReason)
        {
            emailContent = SubwayContext.Current.LookUpValuesRepo.GetContent("EMAIL_DELETE_EVENT");

            if (string.IsNullOrWhiteSpace(emailContent.Content))
                SubwayContext.Current.Logger.Log("Exception Email: EMAIL_DELETE_EVENT is missing.");

            return emailContent.Content.Replace("[recipient]", recipient)
                                    .Replace("[eventname]", eventName)
                                    .Replace("[datestart]", dateStart)
                                    .Replace("[deletereason]", deleteReason);
           
        }
    }
}