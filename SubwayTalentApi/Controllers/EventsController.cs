using SubwayTalent.Contracts;
using SubwayTalent.Core.Exceptions;
using SubwayTalent.Core.Utilities;
using SubwayTalent.Logging;
using SubwayTalentApi.ActionFilters;
using SubwayTalentApi.Helpers;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;

namespace SubwayTalentApi.Controllers
{

    public class EventsController : ApiController
    {

        ResponseModel response = null;

        #region "Web Method(s)"

        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult AddEvent(EventModel eventDetails)
        {
            try
            {
                if (eventDetails.Planners == null || eventDetails.Planners.Count <= 0)
                    throw new Exception("No planner associated");
                if (eventDetails.Talents == null || eventDetails.Talents.Count <= 0)
                    throw new Exception("No talent(s) associated");

                SubwayContext.Current.EventRepo.AddEvent(ConvertSubwayObject.ConvertToEvent(eventDetails));


                response = new ResponseModel
                {
                    Status = Status.Success
                };

                return Ok(response);


            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;

                response = new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                };

                return Ok(response);
            }
        }
        [AuthorizationRequired]
        [HttpPost]
        public Task<ResponseModel> AddEventMultiPart()
        {

            return ProcessEventMultipart(false);

        }
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult GetEventsPlanner(UserModel user)
        {
            if (user == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (string.IsNullOrWhiteSpace(user.UserId))
                throw new SubwayTalentException("UserId is required.");

            var result = SubwayContext.Current.EventRepo.GetEventsPlanner(user.UserId);

            response = new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result == null) ? 0 : result.Count
            };

            return Ok(response);
        }

        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult GetEventsTalent(UserModel user)
        {
            if (user == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (string.IsNullOrWhiteSpace(user.UserId))
                throw new SubwayTalentException("UserId is required.");

            var result = SubwayContext.Current.EventRepo.GetEventsTalent(user.UserId);

            response = new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result == null) ? 0 : result.Count
            };

            return Ok(response);
        }

        /// <summary>
        /// Used in home tab of the talent to get the list of events.
        /// </summary>
        /// <param name="user">not required. if user has value it won't get any events associated to that user</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllEvents(HomeModel homeModel)
        {
            if (homeModel == null)
                throw new SubwayTalentException("Invalid parameters.");
            //if (string.IsNullOrWhiteSpace(homeModel.UserId))
            //    throw new SubwayTalentException("UserId is required.");

            var result = SubwayContext.Current.EventRepo.GetAllEvents(homeModel.UserId);

            //TODO: timezones
            if (homeModel.RemoveExpiredDates == 1)
                result = result.Where(x => x.DateStart >= ((homeModel.CurrentDateTime.ToShortDateString() == "1/1/0001") ? DateTime.Now : homeModel.CurrentDateTime)).ToList();

            response = new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result == null) ? 0 : result.Count
            };

            return Ok(response);
        }

        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult GetPlannerInvites(UserModel user)
        {
            if (user == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (string.IsNullOrWhiteSpace(user.UserId))
                throw new SubwayTalentException("UserId is required.");

            var result = SubwayContext.Current.EventRepo.GetPlannerInvites(user.UserId);

            response = new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result == null) ? 0 : result.Count
            };

            return Ok(response);

        }

        /// <summary>
        /// For invites tab of talent user. This is grouped by event type and you won't see any event name or id, just event type info and then the planer
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult GetTalentInvites(UserModel user)
        {

            if (user == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (string.IsNullOrWhiteSpace(user.UserId))
                throw new SubwayTalentException("UserId is required.");

            var result = SubwayContext.Current.EventRepo.GetTalentInvites(user.UserId);

            response = new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result == null) ? 0 : result.Count
            };

            return Ok(response);

        }
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult AddTalentsToEvent(EventModel eventModel)
        {
            if (eventModel == null)
                throw new SubwayTalentException("Event Details is nothing");

            if (eventModel.Id == 0)
                throw new SubwayTalentException("Please add an event id.");

            if (eventModel.Talents == null || eventModel.Talents.Count == 0)
                throw new SubwayTalentException("Please add talent(s).");

            SubwayContext.Current.EventRepo.AddTalentToEvent(new Event
            {
                Id = eventModel.Id,
                Talents = new List<UserAccount>(
                                    eventModel.Talents.Select(talent =>
                                    {
                                        return new UserAccount
                                        {
                                            UserId = talent.UserId
                                        };
                                    }))
            });

            response = new ResponseModel
            {
                Status = Status.Success
            };

            return Ok(response);

        }


        private string AcceptRejectRequestTalentInvite(Event eventObj, InviteStatus status, string userId, string updatedBy)
        {
            var result = string.Empty;
            var talent = eventObj.Talents.FirstOrDefault(x => x.UserId == userId);

            var talentUpdatedBy = eventObj.Talents.FirstOrDefault(x => x.UserId == updatedBy);
            var plannerUpdatedBy = eventObj.Planners.FirstOrDefault(x => x.UserId == updatedBy);

            switch (status)
            {
                case InviteStatus.Accepted:
                    {
                        //check if the userId is associated to the event.
                        if (talent == null) throw new SubwayTalentException("The userId is not a talent to the event.");
                        //check if the udpatedBy is associated to the event.
                        if (talentUpdatedBy == null && plannerUpdatedBy == null) throw new SubwayTalentException("You do not belong to the event.");

                        //talent accepted
                        if (updatedBy.ToLower() == talent.UserId.ToLower())
                        {
                            if (InviteStatus.Requested == (InviteStatus)talent.Status)
                                throw new SubwayTalentException("You can't accept if your the one who requested for the event.");


                            result = SubwayContext.Current.EventRepo.AcceptRejectRequestTalentInvite(updatedBy, eventObj.Id, (short)status, updatedBy);

                        }
                        //planner accepted request
                        if (updatedBy.ToLower() == eventObj.Planners[0].UserId.ToLower())
                            result = SubwayContext.Current.EventRepo.AcceptRejectRequestTalentInvite(userId, eventObj.Id, (short)status, updatedBy);
                        break;
                    }

                case InviteStatus.Rejected:
                    {
                        //check if the userId is associated to the event.
                        if (talent == null) throw new SubwayTalentException("The userId is not a talent to the event.");
                        //check if the udpatedBy is associated to the event.
                        if (talentUpdatedBy == null && plannerUpdatedBy == null) throw new SubwayTalentException("You do not belong to the event.");

                        //talent rejected
                        if (updatedBy.ToLower() == talent.UserId.ToLower())
                            result = SubwayContext.Current.EventRepo.AcceptRejectRequestTalentInvite(updatedBy, eventObj.Id, (short)status, updatedBy);

                        //planner rejected request
                        if (updatedBy.ToLower() == eventObj.Planners[0].UserId.ToLower())
                            result = SubwayContext.Current.EventRepo.AcceptRejectRequestTalentInvite(userId, eventObj.Id, (short)status, updatedBy);

                        break;
                    }
                case InviteStatus.Invited:
                    {
                        //check if the udpatedBy is associated to the event.
                        if (plannerUpdatedBy == null || plannerUpdatedBy.UserId.ToLower() != updatedBy.ToLower())
                            throw new SubwayTalentException("You do not belong to event or you are not allowed to invite a talent.");
                        result = SubwayContext.Current.EventRepo.AcceptRejectRequestTalentInvite(userId, eventObj.Id, (short)status, updatedBy);

                        break;
                    }
                case InviteStatus.Requested:
                    {
                        result = SubwayContext.Current.EventRepo.AcceptRejectRequestTalentInvite(userId, eventObj.Id, (short)status, updatedBy);
                        break;
                    }
                default:
                    {
                        throw new SubwayTalentException("Invalid status.");
                    }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">Status [0-pending,1-accepted,2-rejected,3-requested],  UserId is the userid of the talent</param>
        /// <returns></returns>
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult AcceptRejectRequestTalentInvite(AcceptRejectModel model)
        {
            if (model == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (model.EventId <= 0)
                throw new SubwayTalentException("Invalid EventId");
            if (string.IsNullOrEmpty(model.UserId))
                throw new SubwayTalentException("Invalid UserId");
            if (model.Status < 0 || model.Status > 3)
                throw new SubwayTalentException("Invalid Status");
            if (string.IsNullOrWhiteSpace(model.UpdatedBy))
                throw new SubwayTalentException("Please supply UpdatedBy parameter");

            var eventObj = SubwayContext.Current.EventRepo.GetEventDetails(model.EventId);

            if (eventObj == null)
                throw new SubwayTalentException("Event doesn't exists.");

            var result = AcceptRejectRequestTalentInvite(eventObj, (InviteStatus)model.Status, model.UserId, model.UpdatedBy);

            if (!string.IsNullOrWhiteSpace(result))
                throw new SubwayTalentException(result);


            var userObj = SubwayContext.Current.UserRepo.GetUserDetails(model.UserId);

            SubwayContext.Current.Notifications.SendStatus((InviteStatus)model.Status, eventObj, userObj, model.UpdatedBy);
            //EmailNotifications.SendStatus((InviteStatus)model.Status, eventObj, userObj, model.UpdatedBy);

            return Ok(new ResponseModel
            {
                Status = Status.Success
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventObj">only used properties are
        /// Id,  Talent with [UserId,TalentName,Rating]
        /// 
        /// {
        ///  "Id": 1,  
        ///  "Talents": [
        ///    {
        ///      "UserId": "newUser1",
        ///      "TalentName": "newUser1",
        ///      "Rating":"1",
        ///      "Comments" : ""
        ///    }],
        ///   "Planners" : [
        ///    {
        ///      "UserId": "Planner1",
        ///    }]
        ///}
        /// </param>
        /// <returns></returns>
        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult RateTalentToEvent(EventModel eventObj)
        {
            if (eventObj == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (eventObj.Talents == null || eventObj.Talents.Count <= 0)
                throw new SubwayTalentException("No talents.");
            if (eventObj.Planners == null || eventObj.Planners.Count <= 0)
                throw new SubwayTalentException("The planner is required please add one.");

            var errorResult = SubwayContext.Current.EventRepo.RateTalentToEvent(ConvertSubwayObject.ConvertToEvent(eventObj));

            return Ok(new ResponseModel
            {
                Status = (errorResult.Count <= 0) ? Status.Success : Status.Failed,
                ErrorMessage = (errorResult.Count <= 0) ? null : string.Format("Can't rate talent(s) [{0}] that hasn't accepted/confirmed to the event or talent(s) doesn't belong to event.", string.Join(",", errorResult))
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventObj">only used properties are
        /// Id,  Planners with [UserId,TalentName,Rating] and talent. Talent is the one who rated the planner
        /// 
        /// {
        ///  "Id": 1,  
        ///  "Planners": [
        ///    {
        ///      "UserId": "Planner1",
        ///      "TalentName": "Planner1",
        ///      "Rating":"4",
        ///      "Comments":"ssdf"
        ///    }],
        ///  "Talents" : [
        ///    {
        ///       "UserId": "newUser2"
        ///    }]
        ///}
        ///</param>
        /// <returns></returns>
        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult RatePlannerToEvent(EventModel eventObj)
        {
            if (eventObj == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (eventObj.Planners == null || eventObj.Planners.Count <= 0)
                throw new SubwayTalentException("No planner(s).");
            if (eventObj.Talents == null || eventObj.Talents.Count <= 0)
                throw new SubwayTalentException("Talent required.");


            var errorResult = SubwayContext.Current.EventRepo.RatePlannerToEvent(ConvertSubwayObject.ConvertToEvent(eventObj));

            return Ok(new ResponseModel
            {
                Status = (errorResult.Count <= 0) ? Status.Success : Status.Failed,
                ErrorMessage = (errorResult.Count <= 0) ? null : string.Format("Can't rate this planner [{0}]. Planner doesn't belong to the event.", string.Join(",", errorResult))
            });

        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public Task<ResponseModel> UpdateEventMultiPart()
        {
            return ProcessEventMultipart(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dropModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DropTalent(DropTalentModel dropModel)
        {
            if (dropModel == null)
                throw new SubwayTalentException("Invalid parameter.");
            if (string.IsNullOrWhiteSpace(dropModel.UserId))
                throw new SubwayTalentException("UserId is required parameter.");
            if (string.IsNullOrWhiteSpace(dropModel.UpdatedBy))
                throw new SubwayTalentException("UpdatedBy is required parameter.");

            var result = SubwayContext.Current.EventRepo.DropTalent(dropModel.EventId, dropModel.UserId, dropModel.Comments, dropModel.Performed, dropModel.UpdatedBy);

            if (!string.IsNullOrWhiteSpace(result))
                throw new SubwayTalentException(result);

            return Ok(new ResponseModel
            {
                Status = Status.Success
            });

        }

        [HttpPost]
        public IHttpActionResult SearchEvent(SearchModel searchModel)
        {
            GeoCoordinate currentLoc = null;

            if (searchModel == null)
                throw new SubwayTalentException("Invalid parameter.");
            if (searchModel.GenreList == null)
                searchModel.GenreList = new List<int>();
            if (searchModel.SkillList == null)
                searchModel.SkillList = new List<int>();

            if (searchModel.DistanceInMeters > 0)
                if (searchModel.CurrentLocation == null && searchModel.CityStateID == null)
                    throw new SubwayTalentException("Please add a location or select a city if searching with proximity");

            if (searchModel.Sort != null)
            {
                if (string.IsNullOrEmpty(searchModel.Sort.Key))
                    throw new SubwayTalentException("Please select a sort catagory.");
            }

            var genreList = string.Join(",", searchModel.GenreList);
            var skillList = string.Join(",", searchModel.SkillList);

            var rawResult = SubwayContext.Current.EventRepo.SearchEvent(searchModel.SearchString, genreList, skillList, searchModel.UserId);
            //var eventsWithLocation = FilterDistance(searchModel, currentLoc, rawResult);
            var eventsWithLocation = LocationHelper.FilterDistace<Event>(searchModel, rawResult, "Event Search");

            //Apply Sorting
            if (searchModel.Sort != null)
                eventsWithLocation = SortHelper.Sort<Event>(searchModel.Sort, eventsWithLocation);

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = eventsWithLocation,
                RecordCount = (eventsWithLocation == null) ? 0 : eventsWithLocation.Count
            });

        }

        [HttpPost]
        public IHttpActionResult GetEventDetails(EventModel eventModel)
        {
            if (eventModel == null)
                throw new SubwayTalentException("Invalid Parameter.");
            if (eventModel.Id == 0)
                throw new SubwayTalentException("Id is required.");

            var result = SubwayContext.Current.EventRepo.GetEventDetails(eventModel.Id);

            return Ok(new ResponseModel
            {
                Status = Status.Success,
                Data = result,
                RecordCount = (result != null) ? 1 : 0
            });
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult DeleteEvent(EventModel eventDetails)
        {
            if (eventDetails == null)
                throw new SubwayTalentException("Invalid parameters.");
            if (eventDetails.Id == 0)
                throw new SubwayTalentException("Invalid EventId.");
            if (string.IsNullOrWhiteSpace(eventDetails.DeleteReason))
                throw new SubwayTalentException("Delete Reason is required.");

            var eventObj = SubwayContext.Current.EventRepo.GetEventDetails(eventDetails.Id);

            if (eventObj == null)
                throw new Exception("Event Doesn't Exists.");

            SubwayContext.Current.EventRepo.DeleteEvent(eventDetails.Id);
            eventObj.DeleteReason = eventDetails.DeleteReason;

            SubwayContext.Current.Notifications.SendEventUpdates(eventObj, 2, eventObj.DeleteReason);
            JavaScriptSerializer sr = new JavaScriptSerializer();

            SubwayContext.Current.Logger.Log("Event Deletion");
            SubwayContext.Current.Logger.Log(sr.Serialize(eventObj));

            return Ok(new ResponseModel
            {
                Status = Status.Success
            });

        }
        #endregion

        #region "Private Method(s)"

        private List<Event> FilterDistance(SearchModel searchModel, GeoCoordinate currentLoc, List<Event> result)
        {
            var eventsWithLocation = new List<Event>();

            if (searchModel.DistanceInMeters > 0)
            {
                foreach (var eventObj in result)
                {
                    if (!string.IsNullOrWhiteSpace(eventObj.Longitude) && !string.IsNullOrWhiteSpace(eventObj.Latitude))
                    {
                        try
                        {
                            //check if the event has a valid location. if not go to the next event.
                            var eventloc = LocationHelper.IsValidLocation(eventObj.Latitude, eventObj.Longitude);

                            //if currentlocation with regards to event is less than the radius.
                            if (eventloc.GetDistanceTo(currentLoc) <= searchModel.DistanceInMeters)
                            {
                                eventObj.Distance = eventloc.GetDistanceTo(currentLoc);
                                eventsWithLocation.Add(eventObj);
                            }

                        }
                        catch (Exception ex)
                        {
                            SubwayContext.Current.Logger.Log(string.Format("event search error. Invalid location for [{0}][{1}]. {2}", eventObj.Name, eventObj.Id.ToString(), ex.Message));
                            continue;
                        }
                    }
                }
            }
            else
                return result;

            return eventsWithLocation;
        }

        private Task<ResponseModel> ProcessEventMultipart(bool isUpdate)
        {
            var folderName = ConfigurationManager.AppSettings["Uploads"];
            var PATH = HttpContext.Current.Server.MapPath("~/" + folderName);
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
            var environment = ConfigurationManager.AppSettings["Environment"];
            if (!Directory.Exists(PATH))
                Directory.CreateDirectory(PATH);
            var errorMessage = string.Empty;

            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new SubwayMultipartFormDataStreamProvider(PATH);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<ResponseModel>(t =>
                {

                    if (t.IsFaulted || t.IsCanceled)
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);

                    //TODO: separate the logic of getting files frrom getting data                   
                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        var filePath = rootUrl + "/" + (string.IsNullOrWhiteSpace(environment) ? string.Empty : environment + "/")
                                       + folderName + "/" + info.Name;

                        return new FileDesc(info.Name, filePath, info.Length / 1024, i.Headers.ContentType.MediaType);
                    });

                    var eventDetails = ParseEventData(streamProvider);

                    if (!isUpdate)
                        AddEventDetails(fileInfo, eventDetails);
                    else
                    {
                        errorMessage = UpdateEventData(PATH, fileInfo, eventDetails);
                        if (!string.IsNullOrWhiteSpace(errorMessage))
                            SubwayContext.Current.Notifications.SendEventUpdates(ConvertSubwayObject.ConvertToEvent(eventDetails), 1, null);

                    }

                    return new ResponseModel
                    {
                        Status = (string.IsNullOrWhiteSpace(errorMessage)) ? Status.Success : Status.Failed,
                        ErrorMessage = (string.IsNullOrWhiteSpace(errorMessage)) ? null : errorMessage
                    };

                });

                return task;
            }
            else
                throw new SubwayTalentException("The request is not multi-part content.");
        }

        private string UpdateEventData(string PATH, IEnumerable<FileDesc> fileInfo, EventModel eventDetails)
        {
            if (fileInfo.ToList().Count > 0)
            {
                if (File.Exists(string.Format("{0}/{1}", PATH, Path.GetFileName(eventDetails.Picture))))
                    File.Delete(string.Format("{0}/{1}", PATH, Path.GetFileName(eventDetails.Picture)));
                eventDetails.Picture = fileInfo.ToList()[0].Path;
            }

            return SubwayContext.Current.EventRepo.Update(ConvertSubwayObject.ConvertToEvent(eventDetails));
        }

        private void AddEventDetails(IEnumerable<FileDesc> fileInfo, EventModel eventDetails)
        {
            if (eventDetails.Planners == null || eventDetails.Planners.Count <= 0)
                throw new Exception("No planner associated");
            //if (eventDetails.Talents == null || eventDetails.Talents.Count <= 0)
            //    throw new Exception("No talent(s) associated");

            if (fileInfo.ToList().Count > 0)
                eventDetails.Picture = fileInfo.ToList()[0].Path;

            //Save to DB
            var eventObj = ConvertSubwayObject.ConvertToEvent(eventDetails);
            SubwayContext.Current.EventRepo.AddEvent(eventObj);

            if (eventObj.Talents != null && eventObj.Talents.Count > 0)
            {
                var planner = SubwayContext.Current.UserRepo.GetUserDetails(eventObj.Planners[0].UserId);

                eventObj.Planners[0] = planner;

                for (var i = 0; i < eventObj.Talents.Count; i++)
                {
                    var userid = eventObj.Talents[i].UserId;

                    eventObj.Talents[i] = SubwayContext.Current.UserRepo.GetUserDetails(userid);

                    SubwayContext.Current.Notifications.SendStatus(InviteStatus.Invited, eventObj, eventObj.Talents[i], planner.UserId);
                    //EmailNotifications.SendStatus(InviteStatus.Pending, eventObj, eventObj.Talents[i], planner.UserId);
                }
            }
        }

        private EventModel ParseEventData(SubwayMultipartFormDataStreamProvider streamProvider)
        {
            var eventModel = new EventModel();
            eventModel.Planners = new List<UserModel>();
            eventModel.Talents = new List<UserModel>();
            eventModel.PreferredGenres = new List<Genre>();
            eventModel.PreferredSkills = new List<Skills>();
            PopulateEventModelProperties(streamProvider, eventModel);

            return eventModel;
        }

        private void PopulateEventModelProperties(SubwayMultipartFormDataStreamProvider streamProvider, EventModel eventModel)
        {
            foreach (var data in streamProvider.FormData)
            {
                PropertyInfo prop = eventModel.GetType().GetProperty(data.ToString(), System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                switch (data.ToString())
                {
                    case "Planners[][UserId]":
                        {
                            var planners = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var planner in planners)
                            {
                                eventModel.Planners.Add(new UserModel
                                {
                                    UserId = planner
                                });
                            }

                            break;
                        }
                    case "Talents[][UserId]":
                        {
                            var talents = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var talent in talents)
                            {
                                eventModel.Talents.Add(new UserModel
                                {
                                    UserId = talent
                                });
                            }
                            break;
                        }
                    case "PreferredSkills[]":
                        {
                            var skills = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var skill in skills)
                            {
                                eventModel.PreferredSkills.Add(new Skills
                                {
                                    Id = Convert.ToInt32(skill)
                                });
                            }
                            break;
                        }
                    case "PreferredGenres[]":
                        {
                            var genres = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var genre in genres)
                            {
                                eventModel.PreferredGenres.Add(new Genre
                                {
                                    Id = Convert.ToInt32(genre)
                                });
                            }
                            break;
                        }
                    case "Type[Id]":
                        {
                            eventModel.Type = new EventTypeModel
                            {
                                Id = Int32.Parse(streamProvider.FormData[data.ToString()])
                            };
                            break;
                        }
                    default:
                        {
                            try
                            {
                                if (streamProvider.FormData[data.ToString()] == null)
                                {
                                    SubwayContext.Current.Logger.Log(string.Format("Warning : Invalid multipart key [{0}]. AddUpdateEvent for [{1}]", data.ToString(), eventModel.Name));
                                }
                                else
                                    prop.SetValue(eventModel, ConvertToProperType(prop.PropertyType.Name, streamProvider.FormData[data.ToString()]));

                            }
                            catch (Exception ex)
                            {
                                SubwayContext.Current.Logger.Log(string.Format("Error : Invalid multipart key [{0}]. AddUpdateEvent for [{1}] or something went wrong with the request.", data.ToString(), eventModel.Name));

                            }
                            break;
                        }
                }
            }
        }

        private dynamic ConvertToProperType(string propertyType, string val)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Int32.Parse(val);
                case "DateTime":
                    return DateTime.Parse(val);
                default:
                    return val;
            }
        }

        #endregion
    }
}
