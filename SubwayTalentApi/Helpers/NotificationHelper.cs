using SubwayTalent.Contracts;
using SubwayTalent.Core;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SubwayTalentApi.Helpers
{
    public class NotificationHelper
    {
        private string body = string.Empty;
        private string subject = string.Empty;

        private string _plannerName = string.Empty;
        private string _talentName = string.Empty;
        private SubwayNotificationPayload _apnPayload;
        private Dictionary<string, object[]> _customObjects;
        private List<UserNotification> _notifications;

        public NotificationHelper()
        {

        }


        public void SendStatus(InviteStatus status, Event eventObj, UserAccount talent, string updatedBy)
        {
            try
            {
                if (eventObj == null || talent == null)
                    throw new Exception("Event details or talent details was not found.");

                switch (status)
                {
                    case InviteStatus.Accepted:
                        {
                            subject = ConfigurationManager.AppSettings["Subject.Event.Accepted"];

                            //talent accepted
                            if (updatedBy.ToLower() == talent.UserId.ToLower())
                            {
                                foreach (var planner in eventObj.Planners)
                                {
                                    _plannerName = planner.FirstName + " " + planner.LastName;
                                    _talentName = talent.FirstName + " " + talent.LastName;
                                    body = EmailContentFactory.GetAcceptedMessage(_plannerName, _talentName, eventObj.Name, eventObj.DateStart.ToString("g"));

                                    if (planner.AllowEmail)
                                        EmailSender.SendMail(string.Empty, planner.Email, body, subject);
                                    if (planner.AllowNotification)
                                        SendNotification(planner.UserId, eventObj.Id,
                                                         talent.UserId, planner.UserId, _talentName,
                                                         _plannerName, eventObj.Name, status, talent.UserId,_talentName);
                                }
                            }

                            //planner accepted request
                            if (updatedBy.ToLower() == eventObj.Planners[0].UserId.ToLower())
                            {
                                _plannerName = eventObj.Planners[0].FirstName + " " + eventObj.Planners[0].LastName;
                                _talentName = talent.FirstName + " " + talent.LastName;
                                body = EmailContentFactory.GetPlannerAcceptedMessage(_plannerName, _talentName, eventObj.Name, eventObj.DateStart.ToString("g"));
                                if (talent.AllowEmail)
                                    EmailSender.SendMail(string.Empty, talent.Email, body, subject);
                                if (talent.AllowNotification)
                                    SendNotification(talent.UserId, eventObj.Id,
                                                      talent.UserId, eventObj.Planners[0].UserId, _talentName,
                                                      _plannerName, eventObj.Name, status,eventObj.Planners[0].UserId, _plannerName);
                            }

                            break;
                        }
                    case InviteStatus.Requested:
                        {
                            subject = ConfigurationManager.AppSettings["Subject.Event.Request"];

                            foreach (var planner in eventObj.Planners)
                            {
                                _plannerName = planner.FirstName + " " + planner.LastName;
                                _talentName = talent.FirstName + " " + talent.LastName;
                                body = EmailContentFactory.GetRequestedMessage(_plannerName, _talentName, eventObj.Name, eventObj.DateStart.ToString("g"));
                                if (planner.AllowEmail)
                                    EmailSender.SendMail(string.Empty, planner.Email, body, subject);

                                if (planner.AllowNotification)
                                    SendNotification(planner.UserId, eventObj.Id,
                                                    talent.UserId, planner.UserId, _talentName,
                                                    _plannerName, eventObj.Name, status, talent.UserId, _talentName);
                            }

                            break;
                        }
                    case InviteStatus.Rejected:
                        {
                            //talent rejected
                            if (updatedBy.ToLower() == talent.UserId.ToLower())
                            {
                                subject = ConfigurationManager.AppSettings["Subject.Event.Talent.Declined"];

                                foreach (var planner in eventObj.Planners)
                                {
                                    _plannerName = planner.FirstName + " " + planner.LastName;
                                    _talentName = talent.FirstName + " " + talent.LastName;
                                    body = EmailContentFactory.GetTalentRejectedMessage(_plannerName, _talentName, eventObj.Name, eventObj.DateStart.ToString("g"));
                                    if (planner.AllowEmail)
                                        EmailSender.SendMail(string.Empty, planner.Email, body, subject);

                                    if (planner.AllowNotification)
                                        SendNotification(planner.UserId, eventObj.Id,
                                                        talent.UserId, planner.UserId, _talentName,
                                                        _plannerName, eventObj.Name, status, talent.UserId, _talentName);
                                }
                            }

                            //planner rejected request
                            if (updatedBy.ToLower() == eventObj.Planners[0].UserId.ToLower())
                            {
                                subject = ConfigurationManager.AppSettings["Subject.Event.Planner.Declined"];
                                _plannerName = eventObj.Planners[0].FirstName + " " + eventObj.Planners[0].LastName;
                                _talentName = talent.FirstName + " " + talent.LastName;
                                body = EmailContentFactory.GetPlannerRejectedMessage(_plannerName, _talentName, eventObj.Name, eventObj.DateStart.ToString("g"));
                                if (talent.AllowEmail)
                                    EmailSender.SendMail(string.Empty, talent.Email, body, subject);
                                if (talent.AllowNotification)
                                    SendNotification(talent.UserId, eventObj.Id,
                                                    talent.UserId, eventObj.Planners[0].UserId, _talentName,
                                                    _plannerName, eventObj.Name, status, eventObj.Planners[0].UserId,_plannerName);
                            }

                            break;
                        }
                    case InviteStatus.Invited:
                        {
                            subject = ConfigurationManager.AppSettings["Subject.Event.Invite"];

                            _plannerName = eventObj.Planners[0].FirstName + " " + eventObj.Planners[0].LastName;
                            _talentName = talent.FirstName + " " + talent.LastName;
                            body = EmailContentFactory.GetInviteMessage(_plannerName, _talentName, eventObj.Name, eventObj.DateStart.ToString("g"));
                            if (talent.AllowEmail)
                                EmailSender.SendMail(string.Empty, talent.Email, body, subject);
                            if (talent.AllowNotification)
                                SendNotification(talent.UserId, eventObj.Id, talent.UserId, eventObj.Planners[0].UserId,
                                                                  talent.TalentName, _plannerName, eventObj.Name,
                                                                  status, eventObj.Planners[0].UserId, _plannerName);
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                var exceptionMessage = "Send email failed. " + ((ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message);
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventObj"></param>
        /// <param name="emailSubject"></param>
        /// <param name="notifAlertMsg"></param>
        /// <param name="updateType">1 for update, 2 for deleting</param>
        public void SendEventUpdates(Event eventObj, short updateType, string deleteReason)
        {
            try
            {
                var userDevices = new List<UserDevice>();

                //Planners
                if (eventObj.Planners != null)
                    eventObj.Planners.ForEach(p =>
                        {
                            SubwayContext.Current.UserRepo.AddUserNotification(p.UserId, eventObj.Id, 0, p.UserId, 2);

                            userDevices = SubwayContext.Current.UserRepo.GetUserDevices(p.UserId);
                            _notifications = SubwayContext.Current.UserRepo.GetUserNotification(p.UserId);


                            if (p.AllowEmail)
                            {
                                body = EmailContentFactory.GetEventUpdateMsg(p.Email, eventObj.Name);
                                subject = "Event Update!!!";

                                //check for type 1 - update, 2 - delete
                                if (updateType == 2)
                                {
                                    body = EmailContentFactory.GetDeleteEventMsg(p.Email, eventObj.Name, eventObj.DateStart.ToString("g"), deleteReason);
                                    subject = "Event Deleted!!!";
                                }
                                EmailSender.SendMail(string.Empty, p.Email, body, subject);
                            }
                            if (p.AllowNotification)
                            {
                                userDevices.ForEach((x) =>
                                   {
                                       if (x.Device == 1)
                                       {

                                           if (string.IsNullOrWhiteSpace(x.DeviceId))
                                           {
                                               SubwayContext.Current.Logger.Log("No device token for " + p.UserId);
                                               return;
                                           }

                                           var plannerName = p.FirstName + " " + p.LastName;
                                           var alertMsg = string.Format("{0} was updated.", eventObj.Name);
                                           if (updateType == 2)
                                               alertMsg = string.Format("{0} was deleted.", eventObj.Name);

                                           _customObjects = new Dictionary<string, object[]>();
                                           _customObjects.Add("status", new object[] { string.Empty });
                                           _customObjects.Add("eventId", new object[] { eventObj.Id });
                                           _customObjects.Add("talentName", new object[] { string.Empty });
                                           _customObjects.Add("plannerName", new object[] { plannerName });
                                           _customObjects.Add("eventName", new object[] { eventObj.Name });
                                           _customObjects.Add("talentId", new object[] { string.Empty });
                                           _customObjects.Add("plannerId", new object[] { p.UserId });

                                           _apnPayload = ConstructAPNPayload(alertMsg, x.DeviceId, _customObjects, _notifications.Count);

                                           SubwayContext.Current.AppleNotification.Send(_apnPayload);


                                       }

                                   });
                            }
                        });

                //for talents
                if (eventObj.Talents != null)
                    eventObj.Talents.ForEach(p =>
                    {
                        SubwayContext.Current.UserRepo.AddUserNotification(p.UserId, eventObj.Id, 0, eventObj.Planners[0].UserId, 2);

                        userDevices = SubwayContext.Current.UserRepo.GetUserDevices(p.UserId);
                        _notifications = SubwayContext.Current.UserRepo.GetUserNotification(p.UserId);

                        if (p.AllowEmail)
                        {
                            body = EmailContentFactory.GetEventUpdateMsg(p.Email, eventObj.Name);
                            subject = "Event Update!!!";
                            //check for type 1 - update, 2 - delete
                            if (updateType == 2)
                            {
                                body = EmailContentFactory.GetDeleteEventMsg(p.Email, eventObj.Name, eventObj.DateStart.ToString("g"), deleteReason);
                                subject = "Event Deleted!!!";
                            }

                            EmailSender.SendMail(string.Empty, p.Email, body, subject);
                        }
                        if (p.AllowNotification)
                        {
                            userDevices.ForEach((x) =>
                            {
                                if (x.Device == 1)
                                {
                                    if (string.IsNullOrWhiteSpace(x.DeviceId))
                                    {
                                        SubwayContext.Current.Logger.Log("No device token for " + p.UserId);
                                        return;
                                    }

                                    var plannerName = (eventObj.Planners != null && eventObj.Planners.Count > 0) ? eventObj.Planners[0].FirstName + " " + eventObj.Planners[0].LastName : string.Empty;
                                    var alertMsg = string.Format("{0} was updated by {1}", eventObj.Name, plannerName);
                                    if (updateType == 2)
                                        alertMsg = string.Format("{0} was deleted by {1}", eventObj.Name, plannerName);

                                    _customObjects = new Dictionary<string, object[]>();
                                    _customObjects.Add("status", new object[] { string.Empty });
                                    _customObjects.Add("eventId", new object[] { eventObj.Id });
                                    _customObjects.Add("talentName", new object[] { string.Empty });
                                    _customObjects.Add("plannerName", new object[] { plannerName });
                                    _customObjects.Add("eventName", new object[] { eventObj.Name });
                                    _customObjects.Add("talentId", new object[] { string.Empty });
                                    _customObjects.Add("plannerId", new object[] { p.UserId });

                                    _apnPayload = ConstructAPNPayload(alertMsg, x.DeviceId, _customObjects, _notifications.Count);

                                    SubwayContext.Current.AppleNotification.Send(_apnPayload);
                                }

                            });
                        }
                    });

            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
            }
        }


        #region "Private Method(s)

        private SubwayNotificationPayload ConstructAPNPayload(string alertMessage, string deviceToken, Dictionary<string, object[]> customObjects, int notifCount)
        {
            var payload = new SubwayNotificationPayload();
            payload.DeviceToken = deviceToken;
            payload.AlertMessage = alertMessage;
            payload.Badge = notifCount;
            payload.Device = DeviceType.Apple;

            if (customObjects != null && customObjects.Count > 0)
            {
                foreach (var item in customObjects)
                {
                    payload.AddCustom(item.Key, item.Value);
                }
            }
            return payload;

        }
        private void SendNotification(string userId, int eventId, string talentId, string plannerId, string talentName,
                                        string plannerName, string eventName, InviteStatus status,
                                        string updatedBy, string updatedByname)
        {
            try
            {
                SubwayContext.Current.UserRepo.AddUserNotification(userId, eventId, (short)status, updatedBy, 1);

                var userDevices = SubwayContext.Current.UserRepo.GetUserDevices(userId);
                _notifications = SubwayContext.Current.UserRepo.GetUserNotification(userId);

                userDevices.ForEach((x) =>
                    {
                        if (x.Device == 1)
                        {
                            if (string.IsNullOrWhiteSpace(x.DeviceId))
                            {
                                SubwayContext.Current.Logger.Log("No device token for " + userId);
                                return;
                            }

                            _customObjects = new Dictionary<string, object[]>();
                            _customObjects.Add("status", new object[] { status.ToString() });
                            _customObjects.Add("eventId", new object[] { eventId });
                            _customObjects.Add("talentName", new object[] { talentName });
                            _customObjects.Add("plannerName", new object[] { plannerName });
                            _customObjects.Add("eventName", new object[] { eventName });
                            _customObjects.Add("talentId", new object[] { talentId });
                            _customObjects.Add("plannerId", new object[] { plannerId });

                            var alertMsg = "You've been {0} by {1}";
                            if (status == InviteStatus.Requested)
                                alertMsg = "{1} {0} to your event.";

                            _apnPayload = ConstructAPNPayload(string.Format(alertMsg, status.ToString(), updatedByname),
                                                    x.DeviceId, _customObjects, _notifications.Count);

                            SubwayContext.Current.AppleNotification.Send(_apnPayload);
                        }

                    });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
            }
        }
        #endregion

    }
}