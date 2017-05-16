using MySql.Data.MySqlClient;
using SubwayTalent.Contracts;
using SubwayTalent.Contracts.Entities;
using SubwayTalent.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Data
{
    public class EventsRepo : IEvent
    {
        private string schema = ConfigurationManager.AppSettings["DBSchema"];
        private string connectionStr = ConfigurationManager.ConnectionStrings["SubwayTalentConnection"].ConnectionString;
        UserAccount _user = null;
        Event _event = null;
        List<Event> _eventList = null;

        public void AddEvent(Event eventObj)
        {

            using (var conn = new MySqlConnection(connectionStr))
            {
                conn.Open();

                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddEvent", conn))
                        {

                            cmd.Transaction = trans;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new MySqlParameter("Name", eventObj.Name));
                            cmd.Parameters.Add(new MySqlParameter("type_id", eventObj.Type.Id));
                            cmd.Parameters.Add(new MySqlParameter("dateCreated", eventObj.DateCreated));
                            cmd.Parameters.Add(new MySqlParameter("dateStarted", eventObj.DateStart));
                            cmd.Parameters.Add(new MySqlParameter("dateEnd", eventObj.DateEnd));
                            cmd.Parameters.Add(new MySqlParameter("description", eventObj.Description));
                            cmd.Parameters.Add(new MySqlParameter("location", eventObj.Location));
                            cmd.Parameters.Add(new MySqlParameter("picture", eventObj.Picture));
                            cmd.Parameters.Add(new MySqlParameter("status", eventObj.Status));
                            cmd.Parameters.Add(new MySqlParameter("title", eventObj.Title));
                            cmd.Parameters.Add(new MySqlParameter("longitude", eventObj.Longitude));
                            cmd.Parameters.Add(new MySqlParameter("latitude", eventObj.Latitude));

                            eventObj.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        AddTalentToEventLocal(eventObj, conn, trans);
                        AddPlannersToEvent(eventObj, conn, trans);
                        AddSkillsToEvent(eventObj, conn, trans);
                        AddGenreToEvent(eventObj, conn, trans);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        trans.Rollback();

                        if (conn.State == ConnectionState.Open)
                            conn.Close();

                        throw ex;
                    }
                }
            }


        }

        private void AddPlannersToEvent(Event eventObj, DbConnection conn, DbTransaction trans)
        {
            var result = 0;

            if (eventObj.Planners != null && eventObj.Planners.Count > 0)
            {
                foreach (var talent in eventObj.Planners)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddEventPlanner", (MySqlConnection)conn))
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        cmd.Transaction = (MySqlTransaction)trans;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new MySqlParameter("event_id", eventObj.Id));
                        cmd.Parameters.Add(new MySqlParameter("userId", talent.UserId));

                        MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default);

                        while (dr.Read())
                        {
                            result = Convert.ToInt16(dr[0]);
                        }
                        dr.Close();
                        //cmd.ExecuteNonQuery();
                        //result = 1;
                    }
                }

                if (result == 0)
                    throw new Exception("The Planner for this event doesn't have any payment methods.");
            }
        }


        public List<Event> GetEventsPlanner(string userId)
        {
            return GetEventInvite(userId, true);
        }

        public List<Event> GetEventsTalent(string userId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                _eventList = new List<Event>();

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetEventsTalent", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();

                    cmd.Parameters.Add(new MySqlParameter("userid", userId));
                    //only confirmed invites
                    cmd.Parameters.Add(new MySqlParameter("inviteStatus", 1));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        _event = new Event
                        {
                            Name = Convert.ToString(dr["Name"]),
                            DateCreated = Convert.ToDateTime(dr["dateCreated"]),
                            DateEnd = Convert.ToDateTime(dr["dateEnd"]),
                            DateStart = Convert.ToDateTime(dr["dateStarted"]),
                            Description = Convert.ToString(dr["description"]),
                            Id = Convert.ToInt16(dr["Id"]),
                            Location = Convert.ToString(dr["location"]),
                            Picture = Convert.ToString(dr["picture"]),
                            Status = (DBNull.Value == dr["status"]) ? 0 : Convert.ToInt16(dr["status"]),
                            Title = Convert.ToString(dr["title"]),
                            Type = new EventType
                            {
                                Id = Convert.ToInt16(dr["typeId"]),
                                Name = Convert.ToString(dr["typeName"])
                            },
                            Latitude = Convert.ToString(dr["latitude"]),
                            Longitude = Convert.ToString(dr["longitude"])
                        };
                        _eventList.Add(_event);
                    }
                    dr.Close();

                    foreach (var ev in _eventList)
                    {
                        PopulateTalentsByEvent(ev, true, false, conn);
                        ev.Planners = PopulatePlannersByEvent(ev.Id, conn);
                        ev.PreferredGenres = GetEventGenreSkill(LookUpValueType.Genres, ev.Id, conn);
                        ev.PreferredSkills = GetEventGenreSkill(LookUpValueType.Skills, ev.Id, conn);
                    }
                }
            }

            return _eventList;
        }

        public List<Event> GetPlannerInvites(string userId)
        {
            return GetEventInvite(userId, false);
        }

        public List<EventType> GetTalentInvites(string userId)
        {
            var _eventTypeList = new List<EventType>();

            using (var conn = new MySqlConnection(connectionStr))
            {

                EventType _eventType = null;
                _eventList = new List<Event>();

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetTalentInvites", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();

                    cmd.Parameters.Add(new MySqlParameter("userid", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var typeId = 0;

                    while (dr.Read())
                    {
                        _eventType = new EventType
                        {
                            Id = Convert.ToInt16(dr["typeId"]),
                            Name = Convert.ToString(dr["typeName"]),

                        };

                        //add to eventType list if new event type
                        if (typeId != _eventType.Id)
                        {
                            typeId = _eventType.Id;

                            _eventType.Events = new List<Event>();
                            _eventType.Events.Add(new Event
                            {
                                Id = Convert.ToInt16(dr["eventId"]),
                                Status = Convert.ToInt16(dr["talentStatus"])
                            });

                            _eventTypeList.Add(_eventType);
                        }
                        else
                        {
                            _eventTypeList.Find(x => x.Id == typeId).Events.Add(new Event
                            {
                                Id = Convert.ToInt16(dr["eventId"]),
                                Status = Convert.ToInt16(dr["talentStatus"])
                            });
                        }
                    }
                    dr.Close();
                }

                //get event details
                foreach (var eventType in _eventTypeList)
                {
                    for (var i = 0; i < eventType.Events.Count; i++)
                    {
                        var statusId = eventType.Events[i].Status;
                        var eventId = eventType.Events[i].Id;

                        eventType.Events[i] = GetEventDetails(eventId);
                        eventType.Events[i].Status = statusId;
                    }

                }

            }
            return _eventTypeList;
        }

        public void AddTalentToEvent(Event eventObj)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                AddTalentToEventLocal(eventObj, conn, null);
            }
        }


        public void AddSkillsToEvent(Event eventObj, DbConnection conn, DbTransaction trans)
        {
            if (eventObj.PreferredSkills != null && eventObj.PreferredSkills.Count > 0)
            {
                foreach (var skill in eventObj.PreferredSkills)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddSkillsToEvent", (MySqlConnection)conn))
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Transaction = (MySqlTransaction)trans;

                        cmd.Parameters.Add(new MySqlParameter("eventId", eventObj.Id));
                        cmd.Parameters.Add(new MySqlParameter("skillId", skill.Id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AddGenreToEvent(Event eventObj, DbConnection conn, DbTransaction trans)
        {
            if (eventObj.PreferredSkills != null && eventObj.PreferredSkills.Count > 0)
            {
                foreach (var genre in eventObj.PreferredGenres)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddGenreToEvent", (MySqlConnection)conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        cmd.Transaction = (MySqlTransaction)trans;

                        cmd.Parameters.Add(new MySqlParameter("eventId", eventObj.Id));
                        cmd.Parameters.Add(new MySqlParameter("genreId", genre.Id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public string AcceptRejectRequestTalentInvite(string userId, int eventId, int status, string updatedBy)
        {
            string errMsgs = null;
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AcceptRejectTalentInvite", conn))
                {

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("talentid", userId));
                    cmd.Parameters.Add(new MySqlParameter("eventid", eventId));
                    cmd.Parameters.Add(new MySqlParameter("statusid", status));
                    cmd.Parameters.Add(new MySqlParameter("updatedBy", updatedBy));


                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        var result = Convert.ToInt16(dr[0]);

                        if (result == 0)
                            errMsgs = "user doesn't exist";

                    }
                    dr.Close();
                }
            }
            return errMsgs;
        }



        public List<Event> GetAllEvents(string userId = null)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                _eventList = new List<Event>();

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetAllEvents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.Parameters.Add(new MySqlParameter("userid", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        _event = new Event
                        {
                            Name = Convert.ToString(dr["Name"]),
                            DateCreated = Convert.ToDateTime(dr["dateCreated"]),
                            DateEnd = Convert.ToDateTime(dr["dateEnd"]),
                            DateStart = Convert.ToDateTime(dr["dateStarted"]),
                            Description = Convert.ToString(dr["description"]),
                            Id = Convert.ToInt16(dr["Id"]),
                            Location = Convert.ToString(dr["location"]),
                            Picture = Convert.ToString(dr["picture"]),
                            Status = (DBNull.Value == dr["status"]) ? 0 : Convert.ToInt16(dr["status"]),
                            Title = Convert.ToString(dr["title"]),
                            Type = new EventType
                            {
                                Id = Convert.ToInt16(dr["typeId"]),
                                Name = Convert.ToString(dr["typeName"])
                            },
                            Latitude = Convert.ToString(dr["latitude"]),
                            Longitude = Convert.ToString(dr["longitude"])
                        };
                        _eventList.Add(_event);
                    }
                    dr.Close();
                }

                //Loop through the eventList
                foreach (var ev in _eventList)
                {
                    //ev.Talents = PopulateTalentsByEvent(ev, confirmedInvite, conn);
                    ev.Planners = PopulatePlannersByEvent(ev.Id, conn);
                    ev.PreferredGenres = GetEventGenreSkill(LookUpValueType.Genres, ev.Id, conn);
                    ev.PreferredSkills = GetEventGenreSkill(LookUpValueType.Skills, ev.Id, conn);

                }
            }

            return _eventList;
        }


        public List<string> RateTalentToEvent(Event eventObj)
        {
            List<string> errMsgs = new List<string>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                foreach (var talent in eventObj.Talents)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_RateTalentToEvent", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (cmd.Connection.State == ConnectionState.Closed)
                            cmd.Connection.Open();

                        cmd.Parameters.Add(new MySqlParameter("talentID", talent.UserId));
                        cmd.Parameters.Add(new MySqlParameter("eventId", eventObj.Id));
                        cmd.Parameters.Add(new MySqlParameter("rating", talent.Rating));
                        cmd.Parameters.Add(new MySqlParameter("feedback", talent.Comments));
                        cmd.Parameters.Add(new MySqlParameter("ratedBy", eventObj.Planners[0].UserId));

                        MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dr.Read())
                        {
                            var result = Convert.ToInt16(dr[0]);

                            if (result == 0)
                                errMsgs.Add(talent.TalentName);

                        }
                        dr.Close();
                    }
                }
            }

            return errMsgs;
        }

        public List<string> RatePlannerToEvent(Event eventObj)
        {
            List<string> errMsgs = new List<string>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                foreach (var planner in eventObj.Planners)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_RatePlannerToEvent", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (cmd.Connection.State == ConnectionState.Closed)
                            cmd.Connection.Open();

                        cmd.Parameters.Add(new MySqlParameter("plannerId", planner.UserId));
                        cmd.Parameters.Add(new MySqlParameter("eventId", eventObj.Id));
                        cmd.Parameters.Add(new MySqlParameter("rating", planner.Rating));
                        cmd.Parameters.Add(new MySqlParameter("feedback", planner.Comments));
                        cmd.Parameters.Add(new MySqlParameter("updatedBy", eventObj.Talents[0].UserId));

                        MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dr.Read())
                        {
                            var result = Convert.ToInt16(dr[0]);

                            if (result == 0)
                                errMsgs.Add(planner.TalentName);

                        }
                        dr.Close();
                    }
                }
            }

            return errMsgs;
        }

        public string Update(Event eventObj)
        {
            int rowsAffected;

            using (var conn = new MySqlConnection(connectionStr))
            {
                conn.Open();

                using (MySqlTransaction trans = conn.BeginTransaction())
                {

                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdateEvent", conn))
                        {

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = trans;
                            cmd.Parameters.Add(new MySqlParameter("eventId", eventObj.Id));
                            cmd.Parameters.Add(new MySqlParameter("event_Name", eventObj.Name));
                            cmd.Parameters.Add(new MySqlParameter("typeid", eventObj.Type.Id));
                            cmd.Parameters.Add(new MySqlParameter("date_Created", eventObj.DateCreated));
                            cmd.Parameters.Add(new MySqlParameter("date_Started", eventObj.DateStart));
                            cmd.Parameters.Add(new MySqlParameter("date_End", eventObj.DateEnd));
                            cmd.Parameters.Add(new MySqlParameter("descr", eventObj.Description));
                            cmd.Parameters.Add(new MySqlParameter("loc", eventObj.Location));
                            cmd.Parameters.Add(new MySqlParameter("pic", eventObj.Picture));
                            cmd.Parameters.Add(new MySqlParameter("status_id", eventObj.Status));
                            cmd.Parameters.Add(new MySqlParameter("title_name", eventObj.Title));
                            cmd.Parameters.Add(new MySqlParameter("lon", eventObj.Longitude));
                            cmd.Parameters.Add(new MySqlParameter("lat", eventObj.Latitude));


                            rowsAffected = cmd.ExecuteNonQuery();

                        }

                        if (rowsAffected == 0)
                        {
                            trans.Rollback();
                            return "event doesn't exist.";
                        }

                        DeleteEventGenreSkills(eventObj.Id, conn, trans);

                        AddSkillsToEvent(eventObj, conn, trans);
                        AddGenreToEvent(eventObj, conn, trans);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            return string.Empty;
        }

        public Event GetEventDetails(int eventId, DbConnection connection = null)
        {
            Event _event = null;

            string errMsgs = string.Empty;

            using (var conn = (connection == null) ? new MySqlConnection(connectionStr) : (MySqlConnection)connection)
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetEventDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("eventId", eventId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {

                        _event = new Event
                        {
                            Name = Convert.ToString(dr["Name"]),
                            DateCreated = Convert.ToDateTime(dr["dateCreated"]),
                            DateEnd = Convert.ToDateTime(dr["dateEnd"]),
                            DateStart = Convert.ToDateTime(dr["dateStarted"]),
                            Description = Convert.ToString(dr["description"]),
                            Id = Convert.ToInt16(dr["Id"]),
                            Location = Convert.ToString(dr["location"]),
                            Picture = Convert.ToString(dr["picture"]),
                            Status = (DBNull.Value == dr["status"]) ? 0 : Convert.ToInt16(dr["status"]),
                            Title = Convert.ToString(dr["title"]),
                            Type = new EventType
                            {
                                Id = Convert.ToInt16(dr["type_Id"]),
                                Name = Convert.ToString(dr["type_name"])
                            },
                            Latitude = Convert.ToString(dr["latitude"]),
                            Longitude = Convert.ToString(dr["longitude"])
                        };
                    }

                    //Planner
                    if (dr.NextResult())
                    {
                        if (_event != null)
                        {
                            _event.Planners = new List<UserAccount>();

                            while (dr.Read())
                            {
                                float rate = 0;
                                float rating = 0;
                                _user = new UserAccount
                                {
                                    LastName = Convert.ToString(dr["LastName"]),
                                    FirstName = Convert.ToString(dr["FirstName"]),
                                    Email = Convert.ToString(dr["Email"]),
                                    UserId = Convert.ToString(dr["UserId"]),
                                    Birthday = Convert.ToDateTime(dr["Birthday"]),
                                    FacebookUser = Convert.ToBoolean(dr["FacebookUser"]),
                                    Bio = Convert.ToString(dr["bio"]),
                                    Rate = (DBNull.Value == dr["rate"]) ? 0 : (float.TryParse(Convert.ToString(dr["rate"]), out rate) ? rate : 0),
                                    Rating = (DBNull.Value == dr["rating"]) ? 0 : (float.TryParse(Convert.ToString(dr["rating"]), out rating) ? rating : 0),
                                    Location = Convert.ToString(dr["location"]),
                                    ProfilePic = Convert.ToString(dr["profilePic"]),
                                    City = Convert.ToString(dr["city"]),
                                    State = Convert.ToString(dr["state_name"]),
                                    Gender = Convert.ToString(dr["gender"]),
                                    MobileNumber = Convert.ToString(dr["mobileNumber"]),
                                    TalentName = Convert.ToString(dr["talentName"]),
                                    ProfilePicTalent = Convert.ToString(dr["profilePicTalent"]),
                                    AllowEmail = (DBNull.Value == dr["allow_email"]) ? false : Convert.ToBoolean(dr["allow_email"]),
                                    AllowNotification = (DBNull.Value == dr["allow_notif"]) ? false : Convert.ToBoolean(dr["allow_notif"]),
                                    Latitude = Convert.ToString(dr["latitude"]),
                                    Longitude = Convert.ToString(dr["longitude"]),
                                    CityStateId = (DBNull.Value == dr["us_cities_id"]) ? 0 : Convert.ToInt32(dr["us_cities_id"]),
                                    StateId = Convert.ToString(dr["state_id"]),
                                    RatingTalent = (DBNull.Value == dr["ratingTalent"]) ? 0 : (float.TryParse(Convert.ToString(dr["ratingTalent"]), out rating) ? rating : 0),

                                };

                                _event.Planners.Add(_user);
                            }
                        }
                    }

                    //event genre
                    if (dr.NextResult())
                    {
                        if (_event != null)
                        {
                            _event.PreferredGenres = new List<LookUpValues>();
                            while (dr.Read())
                            {
                                _event.PreferredGenres.Add(new LookUpValues
                                {
                                    Id = Convert.ToInt16(dr["Id"]),
                                    Name = Convert.ToString(dr["Name"])
                                });

                            }
                        }
                    }

                    //event skills
                    if (dr.NextResult())
                    {
                        if (_event != null)
                        {
                            _event.PreferredSkills = new List<LookUpValues>();
                            while (dr.Read())
                            {
                                _event.PreferredSkills.Add(new LookUpValues
                                {
                                    Id = Convert.ToInt16(dr["Id"]),
                                    Name = Convert.ToString(dr["Name"])
                                });

                            }
                        }
                    }

                    dr.Close();
                }

                if (_event != null)
                    PopulateTalentsByEvent(_event, false, true, conn);
            }



            return _event;
        }

        public string DropTalent(int eventId, string talentId, string comments, bool performed, string updatedBy)
        {
            int rowsAffected;
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DropTalentToEvent", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();


                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("eventId", eventId));
                    cmd.Parameters.Add(new MySqlParameter("talentId", talentId));
                    cmd.Parameters.Add(new MySqlParameter("comments", comments));
                    cmd.Parameters.Add(new MySqlParameter("performed_flg", (performed) ? 1 : 0));
                    cmd.Parameters.Add(new MySqlParameter("updatedBy", updatedBy));

                    rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("user doesn't exist or user doen't belong to the event.");
                    }
                }
            }

            return string.Empty;
        }

        public List<Event> SearchEvent(string searchString, string genreList, string skillList, string userId = null)
        {
            _eventList = new List<Event>();
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_SearchEvents", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();


                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("searchKeyWord", string.IsNullOrWhiteSpace(searchString) ? string.Empty : searchString));
                    cmd.Parameters.Add(new MySqlParameter("genreList", genreList));
                    cmd.Parameters.Add(new MySqlParameter("skillList", skillList));
                    cmd.Parameters.Add(new MySqlParameter("userid", string.IsNullOrWhiteSpace(userId) ? string.Empty : userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        _eventList.Add(new Event
                        {
                            Id = Convert.ToInt16(dr["id"])
                        });
                    }
                    dr.Close();

                }

                for (var i = 0; i < _eventList.Count; i++)
                {
                    var eventId = _eventList[i].Id;
                    _eventList[i] = GetEventDetails(eventId, conn);
                }
            }



            return _eventList;
        }

        public void DeleteEvent(int eventId)
        {

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteEvent", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("eventIdParam", eventId));

                    cmd.ExecuteNonQuery();
                }
            }


        }

        public List<EventPlannerPayment> GetDoneEventsPaymentInfo()
        {
            var listPayment = new List<EventPlannerPayment>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                listPayment = new List<EventPlannerPayment>();

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetDoneEvents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();


                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        listPayment.Add(new EventPlannerPayment
                        {
                            EventPayment = new Payment
                            {
                                Method = new PaymentMethod
                                {
                                    Id = Convert.ToInt32(dr["payment_method_id"]),
                                    Name = Convert.ToString(dr["payment_name"]),
                                    Processor = Convert.ToString(dr["payment_processor"])
                                },
                                PaymentInstrumentId = Convert.ToString(dr["payment_instrument_id"]),
                                RefreshToken = Convert.ToString(dr["refresh_token"]),
                                UserId = Convert.ToString(dr["user_id"])
                            },
                            EventId = Convert.ToInt32(dr["eventId"]),
                            EventPlannerId = Convert.ToInt32(dr["eventPlannerId"]),
                            PaymentStatus = Convert.ToInt16(dr["payment_status"]),
                            TransactionAuthId = Convert.ToString(dr["transaction_auth_id"]),
                            TransactionIdCompleted = Convert.ToString(dr["transaction_id_completed"])
                        });
                    }
                }
            }
            return listPayment;

        }

        #region "Private Method(s)"

        private void DeleteEventGenreSkills(int eventId, DbConnection conn, DbTransaction trans)
        {
            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteEventPreferredSkillsGenre", (MySqlConnection)conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = (MySqlTransaction)trans;
                cmd.Parameters.Add(new MySqlParameter("eventId", eventId));
                cmd.ExecuteNonQuery();
            }
        }

        private void AddTalentToEventLocal(Event eventObj, DbConnection conn, DbTransaction trans)
        {
            if (eventObj.Talents != null && eventObj.Talents.Count > 0)
            {
                foreach (var talent in eventObj.Talents)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddTalentToEvent", (MySqlConnection)conn))
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        if (trans != null)
                            cmd.Transaction = (MySqlTransaction)trans;

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(new MySqlParameter("userIdParam", talent.UserId));
                        cmd.Parameters.Add(new MySqlParameter("eventid", eventObj.Id));

                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }

        private List<Event> GetEventInvite(string userId, bool confirmedInvite)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                _eventList = new List<Event>();

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetEventsPlanner", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();

                    cmd.Parameters.Add(new MySqlParameter("userid", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        _event = new Event
                        {
                            Name = Convert.ToString(dr["Name"]),
                            DateCreated = Convert.ToDateTime(dr["dateCreated"]),
                            DateEnd = Convert.ToDateTime(dr["dateEnd"]),
                            DateStart = Convert.ToDateTime(dr["dateStarted"]),
                            Description = Convert.ToString(dr["description"]),
                            Id = Convert.ToInt16(dr["Id"]),
                            Location = Convert.ToString(dr["location"]),
                            Picture = Convert.ToString(dr["picture"]),
                            Status = (DBNull.Value == dr["status"]) ? 0 : Convert.ToInt16(dr["status"]),
                            Title = Convert.ToString(dr["title"]),
                            Type = new EventType
                            {
                                Id = Convert.ToInt16(dr["typeId"]),
                                Name = Convert.ToString(dr["typeName"])
                            },
                            Longitude = Convert.ToString(dr["longitude"]),
                            Latitude = Convert.ToString(dr["latitude"])

                        };
                        _eventList.Add(_event);
                    }
                    dr.Close();
                }

                //Loop through the eventList
                foreach (var ev in _eventList)
                {
                    PopulateTalentsByEvent(ev, confirmedInvite, false, conn);

                    ev.PreferredGenres = GetEventGenreSkill(LookUpValueType.Genres, ev.Id, conn);
                    ev.PreferredSkills = GetEventGenreSkill(LookUpValueType.Skills, ev.Id, conn);

                }
            }

            return _eventList;
        }

        /// <summary>
        /// Add Talents 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="statusId"></param>
        private void PopulateTalentsByEvent(Event ev, bool confirmedInvite, bool allTalents, MySqlConnection conn)
        {
            var userRepo = new UserAccountRepo();

            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetTalentsByEvent", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();

                cmd.Parameters.Add(new MySqlParameter("eventId", ev.Id));
                //status 1 for confirmed talents.
                cmd.Parameters.Add(new MySqlParameter("inviteStatus", (confirmedInvite) ? "1" : "0"));
                cmd.Parameters.Add(new MySqlParameter("allTalent", (allTalents) ? "1" : "0"));

                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                ev.Talents = new List<UserAccount>();

                while (dr.Read())
                {
                    float rate = 0;
                    float eventRate = 0;
                    float rating = 0;
                    _user = new UserAccount
                    {
                        LastName = Convert.ToString(dr["LastName"]),
                        FirstName = Convert.ToString(dr["FirstName"]),
                        Email = Convert.ToString(dr["Email"]),
                        UserId = Convert.ToString(dr["UserId"]),
                        Birthday = Convert.ToDateTime(dr["Birthday"]),
                        FacebookUser = Convert.ToBoolean(dr["FacebookUser"]),
                        Bio = Convert.ToString(dr["bio"]),
                        Rate = (DBNull.Value == dr["rate"]) ? 0 : (float.TryParse(Convert.ToString(dr["rate"]), out rate) ? rate : 0),
                        Rating = (DBNull.Value == dr["rating"]) ? 0 : (float.TryParse(Convert.ToString(dr["rating"]), out rating) ? rating : 0),
                        Location = Convert.ToString(dr["location"]),
                        ProfilePic = Convert.ToString(dr["profilePic"]),
                        City = Convert.ToString(dr["city"]),
                        State = Convert.ToString(dr["state_name"]),
                        Gender = Convert.ToString(dr["gender"]),
                        MobileNumber = Convert.ToString(dr["mobileNumber"]),
                        TalentName = Convert.ToString(dr["talentName"]),
                        Status = Convert.ToInt16(dr["status_id"]),
                        ProfilePicTalent = Convert.ToString(dr["profilePicTalent"]),
                        AllowEmail = (DBNull.Value == dr["allow_email"]) ? false : Convert.ToBoolean(dr["allow_email"]),
                        AllowNotification = (DBNull.Value == dr["allow_notif"]) ? false : Convert.ToBoolean(dr["allow_notif"]),
                        Latitude = Convert.ToString(dr["latitude"]),
                        Longitude = Convert.ToString(dr["longitude"]),
                        CityStateId = (DBNull.Value == dr["us_cities_id"]) ? 0 : Convert.ToInt32(dr["us_cities_id"]),
                        StateId = Convert.ToString(dr["state_id"]),
                        EventRate = (DBNull.Value == dr["user_rate"]) ? 0 : (float.TryParse(Convert.ToString(dr["user_rate"]), out eventRate) ? eventRate : 0),
                        RatingTalent = (DBNull.Value == dr["ratingTalent"]) ? 0 : (float.TryParse(Convert.ToString(dr["ratingTalent"]), out rating) ? rating : 0),
                        PaymentStatus = (DBNull.Value == dr["payment_status"]) ? Convert.ToInt16(0) : Convert.ToInt16(dr["payment_status"]),
                        PaymentDateUpdate = (DBNull.Value == dr["payment_date_update"]) ? new DateTime() : Convert.ToDateTime(dr["Birthday"])

                    };

                    ev.Talents.Add(_user);
                }
                dr.Close();
            }


            foreach (var talent in ev.Talents)
            {
                var userMedia = userRepo.GetUserExternalMedia(talent.UserId, conn);
                talent.Skills = userRepo.GetUserGenreSkill(LookUpValueType.Skills, talent.UserId, conn);
                talent.Genres = userRepo.GetUserGenreSkill(LookUpValueType.Genres, talent.UserId, conn);
                talent.SoundCloud = userMedia.Where(x => x.ExternalType == "S").ToList();
                talent.Youtube = userMedia.Where(x => x.ExternalType == "Y").ToList();
                talent.PaymentMethod = userRepo.GetPaymentMethods(talent.UserId, true);
            }

        }

        private List<UserAccount> PopulatePlannersByEvent(int eventId, MySqlConnection conn)
        {
            var userRepo = new UserAccountRepo();
            List<UserAccount> userList;
            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetPlannersByEvent", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();

                cmd.Parameters.Add(new MySqlParameter("eventId", eventId));

                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);


                userList = new List<UserAccount>();
                while (dr.Read())
                {
                    float rate = 0;
                    float rating = 0;
                    _user = new UserAccount
                    {
                        LastName = Convert.ToString(dr["LastName"]),
                        FirstName = Convert.ToString(dr["FirstName"]),
                        Email = Convert.ToString(dr["Email"]),
                        UserId = Convert.ToString(dr["UserId"]),
                        Birthday = Convert.ToDateTime(dr["Birthday"]),
                        FacebookUser = Convert.ToBoolean(dr["FacebookUser"]),
                        Bio = Convert.ToString(dr["bio"]),
                        Rate = (DBNull.Value == dr["rate"]) ? 0 : (float.TryParse(Convert.ToString(dr["rate"]), out rate) ? rate : 0),
                        Rating = (DBNull.Value == dr["rating"]) ? 0 : (float.TryParse(Convert.ToString(dr["rating"]), out rating) ? rating : 0),
                        Location = Convert.ToString(dr["location"]),
                        ProfilePic = Convert.ToString(dr["profilePic"]),
                        City = Convert.ToString(dr["city"]),
                        State = Convert.ToString(dr["state_name"]),
                        Gender = Convert.ToString(dr["gender"]),
                        MobileNumber = Convert.ToString(dr["mobileNumber"]),
                        TalentName = Convert.ToString(dr["talentName"]),
                        ProfilePicTalent = Convert.ToString(dr["profilePicTalent"]),
                        AllowEmail = (DBNull.Value == dr["allow_email"]) ? false : Convert.ToBoolean(dr["allow_email"]),
                        AllowNotification = (DBNull.Value == dr["allow_notif"]) ? false : Convert.ToBoolean(dr["allow_notif"]),
                        Latitude = Convert.ToString(dr["latitude"]),
                        Longitude = Convert.ToString(dr["longitude"]),
                        CityStateId = (DBNull.Value == dr["us_cities_id"]) ? 0 : Convert.ToInt32(dr["us_cities_id"]),
                        StateId = Convert.ToString(dr["state_id"])
                    };

                    userList.Add(_user);
                }
                dr.Close();

                foreach (var user in userList)
                {
                    var userMedia = userRepo.GetUserExternalMedia(user.UserId, conn);
                    user.Skills = userRepo.GetUserGenreSkill(LookUpValueType.Skills, user.UserId, conn);
                    user.Genres = userRepo.GetUserGenreSkill(LookUpValueType.Genres, user.UserId, conn);
                    user.SoundCloud = userMedia.Where(x => x.ExternalType == "S").ToList();
                    user.Youtube = userMedia.Where(x => x.ExternalType == "Y").ToList();
                    user.PaymentMethod = userRepo.GetPaymentMethods(user.UserId, true);
                }
            }
            return userList;
        }

        public List<LookUpValues> GetEventGenreSkill(LookUpValueType lookUpType, int eventId, DbConnection conn)
        {
            List<LookUpValues> _lookupValues;

            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetEventPreferred" + lookUpType.ToString(), (MySqlConnection)conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection.Open();

                cmd.Parameters.Add(new MySqlParameter("event_id", eventId));

                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                _lookupValues = new List<LookUpValues>();

                while (dr.Read())
                {
                    _lookupValues.Add(new LookUpValues
                    {
                        Id = Convert.ToInt16(dr["Id"]),
                        Name = Convert.ToString(dr["Name"])
                    });
                }
                dr.Close();
            }

            return _lookupValues;
        }
        #endregion
    }
}
