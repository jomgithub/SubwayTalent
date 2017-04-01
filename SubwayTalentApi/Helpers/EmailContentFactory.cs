using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public class EmailContentFactory
    {
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
            var htmlMessage = string.Empty;

            var htmlFilePath = string.Format("{0}Resources/TalentAcceptedMsg.html", AppDomain.CurrentDomain.BaseDirectory);
            using (StreamReader reader = File.OpenText(htmlFilePath)) 
            {
                htmlMessage = reader.ReadToEnd();  
            }

            htmlMessage = htmlMessage.Replace("[planner]", plannerName)
                            .Replace("[talent]", talentName)
                            .Replace("[event]", eventName)
                            .Replace("[eventdate]", eventdate);
           
            return htmlMessage;
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
            var htmlMessage = string.Empty;

            var htmlFilePath = string.Format("{0}Resources/TalentInviteMsg.html", AppDomain.CurrentDomain.BaseDirectory);
            using (StreamReader reader = File.OpenText(htmlFilePath))  
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[planner]", plannerName)
                           .Replace("[talent]", talentName)
                           .Replace("[event]", eventName)
                           .Replace("[eventdate]", eventdate);

            return htmlMessage;
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
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/PlannerAcceptedMsg.html", AppDomain.CurrentDomain.BaseDirectory);
          
            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[planner]", plannerName)
                           .Replace("[talent]", talentName)
                           .Replace("[event]", eventName)
                           .Replace("[eventdate]", eventdate);

            return htmlMessage;
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
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/RequestToEvent.html", AppDomain.CurrentDomain.BaseDirectory);
           
            using (StreamReader reader = File.OpenText(htmlFilePath))  
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[planner]", plannerName)
                           .Replace("[talent]", talentName)
                           .Replace("[event]", eventName)
                           .Replace("[eventdate]", eventdate);

            return htmlMessage;
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
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/TalentRejectedMsg.html", AppDomain.CurrentDomain.BaseDirectory);
            
            using (StreamReader reader = File.OpenText(htmlFilePath))  
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[planner]", plannerName)
                           .Replace("[talent]", talentName)
                           .Replace("[event]", eventName)
                           .Replace("[eventdate]", eventdate);

            return htmlMessage;
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
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/PlannerRejectedMsg.html", AppDomain.CurrentDomain.BaseDirectory);
          
            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[planner]", plannerName)
                           .Replace("[talent]", talentName)
                           .Replace("[event]", eventName)
                           .Replace("[eventdate]", eventdate);

            return htmlMessage;
        }

        public static string GetForgotPasswordTemplate(string userFullName, string tempPassword)
        {
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/ForgotPassword.html", AppDomain.CurrentDomain.BaseDirectory);

            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[user]", userFullName)
                           .Replace("[password]", tempPassword);

            return htmlMessage;
        }

        public static string GetHelpTemplate(string userFullName)
        {
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/HelpMessage.html", AppDomain.CurrentDomain.BaseDirectory);

            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[Sender]", userFullName);
                         

            return htmlMessage;
        }

        public static string GetHelpToAdminTemplate(string userFullName, string email, string message)
        {
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/HelpToAdmin.html", AppDomain.CurrentDomain.BaseDirectory);

            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[sender]", userFullName)
                                    .Replace("[email]", email)
                                    .Replace("[message]", message);


            return htmlMessage;
        }

        public static string GetEventUpdateMsg(string recipient, string eventName)
        {
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/EventUpdateMsg.html", AppDomain.CurrentDomain.BaseDirectory);

            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[recipient]", recipient)
                                    .Replace("[eventName]", eventName);


            return htmlMessage;
        }

        public static string GetDeleteEventMsg(string recipient, string eventName, string dateStart, string deleteReason)
        {
            var htmlMessage = string.Empty;
            var htmlFilePath = string.Format("{0}Resources/DeleteEventMsg.html", AppDomain.CurrentDomain.BaseDirectory);

            using (StreamReader reader = File.OpenText(htmlFilePath))
            {
                htmlMessage = reader.ReadToEnd();
            }

            htmlMessage = htmlMessage.Replace("[recipient]", recipient)
                                    .Replace("[eventname]", eventName)
                                    .Replace("[datestart]", dateStart)
                                    .Replace("[deletereason]", deleteReason);


            return htmlMessage;
        } 
    }
}