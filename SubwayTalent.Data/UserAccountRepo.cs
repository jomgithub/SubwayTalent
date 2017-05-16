using MySql.Data.MySqlClient;
using SubwayTalent.Contracts;
using SubwayTalent.Contracts.Interfaces;
using SubwayTalent.Core.Exceptions;
using SubwayTalent.Data.Interfaces;
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
    public class UserAccountRepo : IUserAccount
    {
        private string schema = ConfigurationManager.AppSettings["DBSchema"];
        private string connectionStr = ConfigurationManager.ConnectionStrings["SubwayTalentConnection"].ConnectionString;
        private List<UserAccount> _userList;
        private UserAccount _user;

        #region "IUserAccount Method(s)"


        public UserAccount GetUserDetails(string userId, DbConnection connection = null)
        {
            UserAccount user = null;

            using (var conn = (connection == null) ? new MySqlConnection(connectionStr) : (MySqlConnection)connection)
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUserDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("userid", userId));


                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {

                        float rate = 0;
                        float rating = 0;

                        user = new UserAccount
                        {
                            LastName = Convert.ToString(dr["LastName"]),
                            FirstName = Convert.ToString(dr["FirstName"]),
                            Email = Convert.ToString(dr["Email"]),
                            UserId = Convert.ToString(dr["UserId"]),
                            Birthday = Convert.ToDateTime(dr["Birthday"] as DateTime? ?? null),
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
                            RatingTalent = (DBNull.Value == dr["ratingTalent"]) ? 0 : (float.TryParse(Convert.ToString(dr["ratingTalent"]), out rating) ? rating : 0)
                        };
                    }
                    dr.Close();
                }

                //user not found.
                if (user == null)
                    return user;

                var userMedia = GetUserExternalMedia(userId, conn);
                user.Skills = GetUserGenreSkill(LookUpValueType.Skills, userId, conn);
                user.Genres = GetUserGenreSkill(LookUpValueType.Genres, userId, conn);
                user.SoundCloud = userMedia.Where(x => x.ExternalType == "S").ToList();
                user.Youtube = userMedia.Where(x => x.ExternalType == "Y").ToList();
                user.PaymentMethod = GetPaymentMethods(userId, true);
            }
            return user;
        }

        public void AddUser(UserAccount user)
        {

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("userid", user.UserId));
                    cmd.Parameters.Add(new MySqlParameter("firstname", user.FirstName));
                    cmd.Parameters.Add(new MySqlParameter("lastname", user.LastName));
                    cmd.Parameters.Add(new MySqlParameter("email", user.Email));
                    cmd.Parameters.Add(new MySqlParameter("password", user.Password));
                    cmd.Parameters.Add(new MySqlParameter("birthday", user.Birthday));
                    cmd.Parameters.Add(new MySqlParameter("lastloggedindate", user.LastLoggedInDate));
                    cmd.Parameters.Add(new MySqlParameter("lockedoutdate", user.LockedOutDate));
                    cmd.Parameters.Add(new MySqlParameter("facebookuser", user.FacebookUser));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUser(string userid)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserAccount user)
        {
            int rowsAffected = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {
                conn.Open();

                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdateUser", conn))
                        {

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = trans;

                            cmd.Parameters.Add(new MySqlParameter("user_id", user.UserId));
                            cmd.Parameters.Add(new MySqlParameter("bio", user.Bio));
                            cmd.Parameters.Add(new MySqlParameter("location", user.Location));
                            cmd.Parameters.Add(new MySqlParameter("rate", user.Rate));
                            cmd.Parameters.Add(new MySqlParameter("rating", user.Rating));
                            cmd.Parameters.Add(new MySqlParameter("email", user.Email));
                            cmd.Parameters.Add(new MySqlParameter("birthday", user.Birthday));
                            cmd.Parameters.Add(new MySqlParameter("firstname", user.FirstName));
                            cmd.Parameters.Add(new MySqlParameter("lastname", user.LastName));
                            cmd.Parameters.Add(new MySqlParameter("profilePic", user.ProfilePic));
                            cmd.Parameters.Add(new MySqlParameter("cityStateId", user.CityStateId));
                            cmd.Parameters.Add(new MySqlParameter("mobileNumber", user.MobileNumber));
                            cmd.Parameters.Add(new MySqlParameter("gender", user.Gender));
                            cmd.Parameters.Add(new MySqlParameter("talentName", user.TalentName));
                            cmd.Parameters.Add(new MySqlParameter("profile_Pic_Talent", user.ProfilePicTalent));

                            rowsAffected = cmd.ExecuteNonQuery();
                        }

                        if (rowsAffected == 0)
                        {
                            trans.Rollback();
                            throw new Exception("user doesn't exist.");
                        }

                        DeleteUserGenres(user, conn, trans);
                        AddUserGenre(user, conn, trans);

                        DeleteUserSkills(user, conn, trans);
                        AddUserSkills(user, conn, trans);


                        //if (user.SoundCloud != null && user.SoundCloud.Count > 0)
                        DeleteExternalMedia(user, "S", conn, trans);
                        //if (user.Youtube != null && user.Youtube.Count > 0)
                        DeleteExternalMedia(user, "Y", conn, trans);

                        AddExternalMedia(user, conn, trans);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public List<UserAccount> GetAllUsers(string userId)
        {
            UserAccount user;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetAllUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    //cmd.Parameters.Add(new MySqlParameter("userType", (userType.ToString() == "Talent") ? "T" : "P"));
                    cmd.Parameters.Add(new MySqlParameter("user_id", userId));


                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    _userList = new List<UserAccount>();

                    while (dr.Read())
                    {
                        float rate = 0;
                        float rating = 0;

                        user = new UserAccount
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
                            RatingTalent = (DBNull.Value == dr["ratingTalent"]) ? 0 : (float.TryParse(Convert.ToString(dr["ratingTalent"]), out rating) ? rating : 0)
                        };


                        _userList.Add(user);
                    }
                    dr.Close();
                }

                foreach (var userAccount in _userList)
                {
                    var userMedia = GetUserExternalMedia(userAccount.UserId, conn);
                    userAccount.Skills = GetUserGenreSkill(LookUpValueType.Skills, userAccount.UserId, conn);
                    userAccount.Genres = GetUserGenreSkill(LookUpValueType.Genres, userAccount.UserId, conn);
                    userAccount.SoundCloud = userMedia.Where(x => x.ExternalType == "S").ToList();
                    userAccount.Youtube = userMedia.Where(x => x.ExternalType == "Y").ToList();
                    userAccount.PaymentMethod = GetPaymentMethods(userAccount.UserId, true);
                }
            }


            return _userList;
        }


        public UserAccount LoginUser(string userid, string password)
        {

            UserAccount user = null;
            var userExists = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_LoginUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("user_id", userid));
                    cmd.Parameters.Add(new MySqlParameter("passwordParam", password));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        user = new UserAccount();

                        user.LastName = Convert.ToString(dr["LastName"]);
                        user.FirstName = Convert.ToString(dr["FirstName"]);
                        user.Email = Convert.ToString(dr["Email"]);
                        user.UserId = Convert.ToString(dr["UserId"]);
                        user.Birthday = Convert.ToDateTime(dr["Birthday"] as DateTime? ?? null); 
                        user.LockedOutDate = Convert.ToDateTime(dr["LockedOutDate"] as DateTime? ?? null);
                        user.LastLoggedInDate = Convert.ToDateTime(dr["LastLoggedInDate"] as DateTime? ?? null);
                        user.FacebookUser = Convert.ToBoolean(dr["FacebookUser"]);
                    }

                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            userExists = Convert.ToInt16(dr["userFound"]);
                        }
                    }

                    dr.Close();
                }
            }

            if (userExists == 1 && user == null)
                throw new SubwayTalentException("Incorrect username or password. Please try again");


            return user;

        }

        public void AddFile(string userId, string fileType, string filePath, string fileName, string thumbPath)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddUserFile", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("user_id", userId));
                    cmd.Parameters.Add(new MySqlParameter("file_path", filePath));
                    cmd.Parameters.Add(new MySqlParameter("file_type", fileType));
                    cmd.Parameters.Add(new MySqlParameter("name", fileName));
                    cmd.Parameters.Add(new MySqlParameter("thumb_path", thumbPath));

                    cmd.ExecuteNonQuery();

                }
            }
        }


        public List<Files> GetFiles(string userId)
        {
            List<Files> files;
            Files file;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUserFiles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("userid", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    files = new List<Files>();
                    while (dr.Read())
                    {
                        file = new Files
                        {
                            FileType = Convert.ToString(dr["file_type"]),
                            Name = Convert.ToString(dr["name"]),
                            Path = Convert.ToString(dr["file_path"]),
                            FileId = Convert.ToInt32(dr["id"]),
                            ThumbNailPath = Convert.ToString(dr["thumbnail_path"])
                        };

                        files.Add(file);
                    }
                    dr.Close();
                }
            }

            return files;
        }


        public string DeleteUserFile(string userId, int id, string fileName)
        {
            var errMsgs = string.Empty;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteUserFile", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("userId", userId));
                    cmd.Parameters.Add(new MySqlParameter("file_id", id));
                    cmd.Parameters.Add(new MySqlParameter("fileName", fileName));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        var result = Convert.ToInt16(dr[0]);

                        if (result == 0)
                            errMsgs = "file doesn't exist";

                    }
                    dr.Close();

                }
            }
            return errMsgs;
        }

        public PlannerCounts GetPlannerCounts(string userId)
        {
            PlannerCounts plannerCounts = null;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetPlannerTotals", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("userId", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        plannerCounts = new PlannerCounts
                        {
                            Booked = Convert.ToInt16(dr["bookedcount"]),
                            Invites = Convert.ToInt16(dr["invitecount"]),
                            Closed = Convert.ToInt16(dr["closecount"])

                        };
                    }
                    dr.Close();
                }
            }

            return plannerCounts;
        }

        public TalentCounts GetTalentCounts(string userId)
        {
            TalentCounts plannerCounts = null;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetTalentTotals", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("userId", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        plannerCounts = new TalentCounts
                        {
                            Leads = Convert.ToInt16(dr["leadcount"]),
                            Booked = Convert.ToInt16(dr["bookedcount"]),
                            Closed = Convert.ToInt16(dr["closecount"])

                        };
                    }
                    dr.Close();
                }
            }

            return plannerCounts;

        }

        public List<UserAccount> SearchTalent(string searchString, string genreList, string skillList, string userId = null)
        {

            _userList = new List<UserAccount>();
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_SearchTalent", conn))
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
                        _userList.Add(new UserAccount
                        {
                            UserId = Convert.ToString(dr["userId"])
                        });
                    }
                    dr.Close();

                }

                for (var i = 0; i < _userList.Count; i++)
                {
                    var currentUserId = _userList[i].UserId;
                    _userList[i] = GetUserDetails(currentUserId);
                }
            }



            return _userList;
        }


        public List<LookUpValues> GetUserGenreSkill(LookUpValueType lookUpType, string userId, DbConnection conn)
        {
            List<LookUpValues> _lookupValues;

            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUser" + lookUpType.ToString(), (MySqlConnection)conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();

                cmd.Parameters.Add(new MySqlParameter("user_id", userId));

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

        public List<ExternalMedia> GetUserExternalMedia(string userId, DbConnection conn)
        {
            List<ExternalMedia> _lookupValues;

            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetExternalMedia", (MySqlConnection)conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();

                cmd.Parameters.Add(new MySqlParameter("userId", userId));

                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                _lookupValues = new List<ExternalMedia>();

                while (dr.Read())
                {
                    _lookupValues.Add(new ExternalMedia
                    {
                        Id = Convert.ToInt16(dr["Id"]),
                        Name = Convert.ToString(dr["name"]),
                        ThumbnailUrl = Convert.ToString(dr["thumbnail_url"]),
                        Url = Convert.ToString(dr["url"]),
                        ExternalType = Convert.ToString(dr["type"])
                    });
                }
                dr.Close();
            }

            return _lookupValues;
        }


        public void UpdateUserFile(string userId, string fileName, string fileType)
        {
            var rowsAffected = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdateUserFile", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));
                    cmd.Parameters.Add(new MySqlParameter("fileType", fileType));
                    cmd.Parameters.Add(new MySqlParameter("fileName", fileName));

                    rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new Exception("file not found.");
                }
            }
        }

        public void SetProfilePic(string userId, int fileId, string perpective)
        {
            var rowsAffected = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_SetProfilePic", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.Parameters.Add(new MySqlParameter("fileId", fileId));
                    cmd.Parameters.Add(new MySqlParameter("user_Id_param", userId));
                    cmd.Parameters.Add(new MySqlParameter("perspective", perpective));

                    rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new Exception("file Id not found for user.");
                }
            }
        }


        public void ChangePassword(string oldPassword, string newPassword, string userId)
        {
            var rowsAffected = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_ChangePassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.Parameters.Add(new MySqlParameter("oldPassword", oldPassword));
                    cmd.Parameters.Add(new MySqlParameter("newPassword", newPassword));
                    cmd.Parameters.Add(new MySqlParameter("user_id", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        var result = Convert.ToInt16(dr[0]);

                        if (result == 0)
                            throw new Exception("Current password and user didn't matched.");

                    }
                    dr.Close();


                }
            }
        }


        public List<RatingComments> GetUserRatingsFeedback(string userId, string userType)
        {
            var commentList = new List<RatingComments>();
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUserComments", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.Parameters.Add(new MySqlParameter("userIdParam", userId));
                    cmd.Parameters.Add(new MySqlParameter("userType", userType));


                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        commentList.Add(new RatingComments
                        {
                            Comment = Convert.ToString(dr["comments"]),
                            DateRated = Convert.ToDateTime(dr["rated_date"]).ToString("d"),
                            RatedBy = Convert.ToString(dr["rated_by"]),
                            Rating = Convert.ToString(dr["user_rating"]),
                            Eventname = Convert.ToString(dr["name"]),
                            RatedByName = Convert.ToString(dr["ratedBy_name"]),
                            ProfilePic = Convert.ToString(dr["profilePicture"])
                        });
                    }
                    dr.Close();


                }
            }

            return commentList;
        }

        public void AddDeviceID(string userId, string deviceId, Int16 deviceType)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddDeviceID", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userIdParam", userId));
                    cmd.Parameters.Add(new MySqlParameter("deviceID", deviceId));
                    cmd.Parameters.Add(new MySqlParameter("deviceType", deviceType));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveDeviceID(string userId, string deviceId)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_RemoveDeviceId", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));
                    cmd.Parameters.Add(new MySqlParameter("deviceId", deviceId));

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<UserDevice> GetUserDevices(string userId)
        {
            var userDevices = new List<UserDevice>();
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUserDevices", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        userDevices.Add(new UserDevice
                        {
                            DeviceId = Convert.ToString(dr["device_id"]),
                            Device = Convert.ToInt16(dr["device_type"]),
                            UserId = Convert.ToString(dr["user_id"])
                        });
                    }
                    dr.Close();
                }
            }
            return userDevices;
        }

        public void ChangeSettings(short allowNotif, short allowEmail, string userId)
        {
            int rowsAffected;

            using (var conn = new MySqlConnection(connectionStr))
            {

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_ChangeSettings", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("user_id", userId));
                    cmd.Parameters.Add(new MySqlParameter("allowNotif", allowNotif));
                    cmd.Parameters.Add(new MySqlParameter("allowEmail", allowEmail));

                    rowsAffected = cmd.ExecuteNonQuery();

                }

                if (rowsAffected == 0)
                    throw new Exception("UserId not found.");
            }
        }

        public void AddUserNotification(string userId, int eventId, short statusId, string updatedBy, Int16 notifType)
        {
            int rowsAffected = 0;

            using (var conn = new MySqlConnection(connectionStr))
            {

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddNotification", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userIdParam", userId));
                    cmd.Parameters.Add(new MySqlParameter("eventId", eventId));
                    cmd.Parameters.Add(new MySqlParameter("statusId", statusId));
                    cmd.Parameters.Add(new MySqlParameter("updatedBy", updatedBy));
                    cmd.Parameters.Add(new MySqlParameter("notifType", notifType));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        rowsAffected = Convert.ToInt16(dr["statExists"]);

                    }
                    dr.Close();

                }

                if (rowsAffected == 1)
                    throw new Exception("The status posted already exists.");
            }
        }

        public List<UserNotification> GetUserNotification(string userId)
        {
            var userNotification = new List<UserNotification>();
            EventsRepo eventObj = new EventsRepo();
            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUserNotifications", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));
                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        userNotification.Add(new UserNotification
                        {
                            EventId = Convert.ToInt32(dr["event_id"]),
                            Id = Convert.ToInt32(dr["id"]),
                            StatusId = Convert.ToInt16(dr["status_id"]),
                            UserId = Convert.ToString(dr["user_id"]),
                            UpdatedBy = Convert.ToString(dr["updated_by"]),
                            NotificationType = (DBNull.Value == dr["notif_type"]) ? 0 : (NotificationType)Convert.ToInt16(dr["notif_type"])

                        });
                    }
                    dr.Close();
                }
            }

            foreach (var item in userNotification)
            {
                item.EventDetails = eventObj.GetEventDetails(item.EventId);
                item.UpdatedByInfo = this.GetUserDetails(item.UpdatedBy);
                item.UserInfo = this.GetUserDetails(item.UserId);
            }
            return userNotification;
        }

        public void DeleteUserNotification(int notificationId)
        {
            int rowsAffected;

            using (var conn = new MySqlConnection(connectionStr))
            {

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteUserNotifications", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("notificationId", notificationId));

                    rowsAffected = cmd.ExecuteNonQuery();
                }

                if (rowsAffected == 0)
                    throw new Exception("Notification doesn't exists. ");
            }
        }

        public void UpdatePassword(string userId, string password)
        {
            int rowsAffected;

            using (var conn = new MySqlConnection(connectionStr))
            {

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_UpdatePassword", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userIdParam", userId));
                    cmd.Parameters.Add(new MySqlParameter("passwordParam", password));

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddHelp(string userId, string senderName, string senderEmail, string subject, string message)
        {
            int rowsAffected;

            using (var conn = new MySqlConnection(connectionStr))
            {

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddHelp", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));
                    cmd.Parameters.Add(new MySqlParameter("sender_Name", senderName));
                    cmd.Parameters.Add(new MySqlParameter("sender_Email", senderEmail));
                    cmd.Parameters.Add(new MySqlParameter("subjectParam", subject));
                    cmd.Parameters.Add(new MySqlParameter("messageParam", message));

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
        }


        public List<Payment> GetPaymentMethods(string userId, bool viewing)
        {
            List<Payment> paymentMethodList = new List<Payment>();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetUserPaymentMethods", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    cmd.Parameters.Add(new MySqlParameter("userId", userId));


                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        paymentMethodList.Add(new Payment
                         {
                             Method = new Contracts.Entities.PaymentMethod
                             {
                                 Id = Convert.ToInt32(dr["payment_method_id"]),
                                 Name = Convert.ToString(dr["payment_name"]),
                                 Processor = (viewing) ? string.Empty : Convert.ToString(dr["payment_processor"])
                             },
                             PaymentInstrumentId = Convert.ToString(dr["payment_instrument_id"]),
                             UserId = Convert.ToString(dr["user_id"]),
                             RefreshToken = (viewing) ? string.Empty : Convert.ToString(dr["refresh_token"]),
                             CardType = Convert.ToString(dr["card_type"]),
                             MaskedCardNumber = Convert.ToString(dr["masked_card_no"])
                         });
                    }
                }
            }

            return paymentMethodList;

        }

        public void AddPaymentMethod(string userId, string refreshToken, string paymentInstrumentId, Int16 paymentMethodId, string maskedCardNo, string cardType)
        {
            using (var conn = new MySqlConnection(connectionStr))
            {

                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddPlannerPaymentMethod", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));
                    cmd.Parameters.Add(new MySqlParameter("refreshToken", refreshToken));
                    cmd.Parameters.Add(new MySqlParameter("paymentMethodId", paymentMethodId));
                    cmd.Parameters.Add(new MySqlParameter("paymentInstrumentId", paymentInstrumentId));
                    cmd.Parameters.Add(new MySqlParameter("maskedCardNo", maskedCardNo));
                    cmd.Parameters.Add(new MySqlParameter("cardType", cardType));

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<Event> GetTalentPendingPayments(string userId)
        {
            var eventList = new List<Event>();
            var _event = new Event();
            var _talent = new UserAccount();
            var _planner = new UserAccount();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetTalentPendingPayments", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        float eventRate = 0;
                        float plannerRating = 0;

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
                            Longitude = Convert.ToString(dr["longitude"]),
                            Talents = new List<UserAccount>()
                             {
                                 new UserAccount
                                 {
                                      EventRate = (DBNull.Value == dr["user_rate"]) ? 0 : (float.TryParse(Convert.ToString(dr["user_rate"]), out eventRate) ? eventRate : 0),                                   
                                      PaymentStatus = (DBNull.Value == dr["payment_status"]) ? Convert.ToInt16(0) : Convert.ToInt16(dr["payment_status"]),
                                      PaymentDateUpdate = (DBNull.Value == dr["payment_date_update"]) ? new DateTime() : Convert.ToDateTime(dr["Birthday"])
                                 
                                 }
                             },
                            Planners = new List<UserAccount>()
                              {
                                  new UserAccount
                                  {
                                       
                                       Rating = (DBNull.Value == dr["plannerRating"]) ? 0 : (float.TryParse(Convert.ToString(dr["plannerRating"]), out plannerRating) ? plannerRating : 0),
                                       LastName = Convert.ToString(dr["LastName"]),
                                        FirstName = Convert.ToString(dr["FirstName"]),
                                        Email = Convert.ToString(dr["Email"]),
                                        UserId = Convert.ToString(dr["UserId"])
                                  }
                              }

                        };
                        eventList.Add(_event);

                        _talent = _event.Talents[0];
                        _planner = _event.Planners[0];
                    }



                    dr.Close();
                }

                //Loop through the eventList
                var eventsRepo = new EventsRepo();
                foreach (var ev in eventList)
                {
                    ev.PreferredGenres = eventsRepo.GetEventGenreSkill(LookUpValueType.Genres, ev.Id, conn);
                    ev.PreferredSkills = eventsRepo.GetEventGenreSkill(LookUpValueType.Skills, ev.Id, conn);

                    ev.Talents[0] = GetUserDetails(userId, conn);
                    ev.Talents[0].EventRate = _talent.EventRate;
                    ev.Talents[0].PaymentStatus = _talent.PaymentStatus;
                    ev.Talents[0].PaymentDateUpdate = _talent.PaymentDateUpdate;

                    ev.Planners[0] = GetUserDetails(_planner.UserId, conn);
                    ev.Planners[0].Rating = _planner.Rating;

                }
            }



            return eventList;
        }


        public List<Event> GetPlannerPendingPayments(string userId)
        {
            var eventList = new List<Event>();
            var _event = new Event();
            var _talent = new UserAccount();
            var _planner = new UserAccount();

            using (var conn = new MySqlConnection(connectionStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_GetPlannerPendingPayments", conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new MySqlParameter("userId", userId));

                    MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        float eventRate = 0;
                        float talentRating = 0;

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
                            Longitude = Convert.ToString(dr["longitude"]),
                            Talents = new List<UserAccount>()
                             {
                                 new UserAccount
                                 {
                                     Rating = (DBNull.Value == dr["user_rating"]) ? 0 : (float.TryParse(Convert.ToString(dr["user_rating"]), out talentRating) ? talentRating : 0),                                       
                                     LastName = Convert.ToString(dr["LastName"]),
                                     FirstName = Convert.ToString(dr["FirstName"]),
                                     Email = Convert.ToString(dr["Email"]),
                                     TalentName = Convert.ToString(dr["talentName"]),
                                     UserId = Convert.ToString(dr["UserId"]), 
                                     EventRate = (DBNull.Value == dr["user_rate"]) ? 0 : (float.TryParse(Convert.ToString(dr["user_rate"]), out eventRate) ? eventRate : 0),                                   
                                     PaymentStatus = (DBNull.Value == dr["payment_status"]) ? Convert.ToInt16(0) : Convert.ToInt16(dr["payment_status"]),
                                     PaymentDateUpdate = (DBNull.Value == dr["payment_date_update"]) ? new DateTime() : Convert.ToDateTime(dr["Birthday"])
                                 
                                 }
                             }

                        };
                        eventList.Add(_event);
                        _talent = _event.Talents[0];
                       
                    }

                    dr.Close();
                }

                //Loop through the eventList
                var eventsRepo = new EventsRepo();
                foreach (var ev in eventList)
                {
                    ev.PreferredGenres = eventsRepo.GetEventGenreSkill(LookUpValueType.Genres, ev.Id, conn);
                    ev.PreferredSkills = eventsRepo.GetEventGenreSkill(LookUpValueType.Skills, ev.Id, conn);

                    ev.Talents[0] = GetUserDetails(userId, conn);
                    ev.Talents[0].EventRate = _talent.EventRate;
                    ev.Talents[0].PaymentStatus = _talent.PaymentStatus;
                    ev.Talents[0].PaymentDateUpdate = _talent.PaymentDateUpdate;                  

                }
            }
            return eventList;
        }
        #endregion

        #region "Private Method(s)"
        private void DeleteUserSkills(UserAccount userObj, DbConnection conn, DbTransaction trans)
        {
            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteTalentSkills", (MySqlConnection)conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = (MySqlTransaction)trans;
                cmd.Parameters.Add(new MySqlParameter("userid", userObj.UserId));
                cmd.ExecuteNonQuery();
            }

        }

        private void DeleteUserGenres(UserAccount userObj, DbConnection conn, DbTransaction trans)
        {
            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteTalentGenres", (MySqlConnection)conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = (MySqlTransaction)trans;
                cmd.Parameters.Add(new MySqlParameter("userid", userObj.UserId));
                cmd.ExecuteNonQuery();
            }
        }

        private void AddUserSkills(UserAccount userObj, DbConnection conn, DbTransaction trans)
        {
            if (userObj.Skills != null && userObj.Skills.Count > 0)
            {
                foreach (var skill in userObj.Skills)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddTalentSkill", (MySqlConnection)conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Transaction = (MySqlTransaction)trans;

                        cmd.Parameters.Add(new MySqlParameter("user_id", userObj.UserId));
                        cmd.Parameters.Add(new MySqlParameter("skill_id", skill.Id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void AddUserGenre(UserAccount userObj, DbConnection conn, DbTransaction trans)
        {
            if (userObj.Genres != null && userObj.Genres.Count > 0)
            {
                foreach (var genre in userObj.Genres)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddTalentGenre", (MySqlConnection)conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Transaction = (MySqlTransaction)trans;

                        cmd.Parameters.Add(new MySqlParameter("user_id", userObj.UserId));
                        cmd.Parameters.Add(new MySqlParameter("genre_id", genre.Id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }



        private void DeleteExternalMedia(UserAccount userObj, string mediaType, DbConnection conn, DbTransaction trans)
        {
            using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_DeleteExternalMedia", (MySqlConnection)conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = (MySqlTransaction)trans;
                cmd.Parameters.Add(new MySqlParameter("userId", userObj.UserId));
                cmd.Parameters.Add(new MySqlParameter("media_type", mediaType));
                cmd.ExecuteNonQuery();
            }
        }

        private void AddExternalMedia(UserAccount userObj, DbConnection conn, DbTransaction trans)
        {
            if (userObj.SoundCloud != null && userObj.SoundCloud.Count > 0)
            {
                foreach (var item in userObj.SoundCloud)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddUserExternalMedia", (MySqlConnection)conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Transaction = (MySqlTransaction)trans;

                        cmd.Parameters.Add(new MySqlParameter("userId", userObj.UserId));
                        cmd.Parameters.Add(new MySqlParameter("name", item.Name));
                        cmd.Parameters.Add(new MySqlParameter("url", item.Url));
                        cmd.Parameters.Add(new MySqlParameter("thumbUrl", item.ThumbnailUrl));
                        cmd.Parameters.Add(new MySqlParameter("media_type", "S"));

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            if (userObj.Youtube != null && userObj.Youtube.Count > 0)
            {
                foreach (var item in userObj.Youtube)
                {
                    using (MySqlCommand cmd = new MySqlCommand(schema + ".spSubway_AddUserExternalMedia", (MySqlConnection)conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Transaction = (MySqlTransaction)trans;

                        cmd.Parameters.Add(new MySqlParameter("userId", userObj.UserId));
                        cmd.Parameters.Add(new MySqlParameter("name", item.Name));
                        cmd.Parameters.Add(new MySqlParameter("url", item.Url));
                        cmd.Parameters.Add(new MySqlParameter("thumbUrl", item.ThumbnailUrl));
                        cmd.Parameters.Add(new MySqlParameter("media_type", "Y"));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion





    }
}
