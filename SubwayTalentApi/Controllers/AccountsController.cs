using SubwayTalent.Contracts;
using SubwayTalent.Core;
using SubwayTalent.Core.Utilities;
using SubwayTalentApi.ActionFilters;
using SubwayTalentApi.Facebook;
using SubwayTalentApi.Filters;
using SubwayTalentApi.Helpers;
using SubwayTalentApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SubwayTalentApi.Controllers
{
    
    public class AccountsController : SubwayBaseController
    {

        /// <summary>
        /// Normal or Facebook Login
        /// </summary>
        /// <param name="userDetails">If AccessToken was supplied, this will automatically be a Facebook Login. The properties that are used are:
        /// {
        ///   "AccessToken":null,
        ///   "UserId":"newUser",
        ///   "Password":"12345",
        ///   "Device" : 1,   (1 - IOS, 2-Android)
        ///   "DeviceToken":null
        /// }
        /// </param>
        /// <returns>the return</returns> 
        [ApiAuthenticationFilter]
        [HttpPost]
        public IHttpActionResult Login(UserModel userDetails)
        {
            try
            {

                var result = LoginHelper.Authenticate(userDetails);
                if (result == null)
                    throw new Exception("failed to login.");
                
                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = result
                });

            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// This is used mainly for signing up users
        /// </summary>
        /// <param name="userDetails">If the user won't use facebook login, the properties that are used are:
        /// {
        ///       "AccessToken":null,
        ///       "UserId":"newUser3",
        ///       "Password":"12345",
        ///       "FirstName":"New",
        ///       "LastName":"User",
        ///        "Email":"newuser@user.com",
        ///       "Birthday":"12/12/2017"  
        ///    }
        /// 
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddUser(UserModel userDetails)
        {
            try
            {
                var errors = ValidateNewUser(userDetails);

                if (errors.Count > 0)
                    return Ok(new ResponseModel
                    {
                        Status = Status.Failed,
                        ErrorMessage = string.Join(" ", errors.ToArray())
                    });


                var existingUser = SubwayContext.Current.UserRepo.GetUserDetails(userDetails.UserId);
                if (existingUser != null)
                    return Ok(new ResponseModel
                    {
                        Status = Status.Failed,
                        ErrorMessage = "user already exists."
                    });

                SubwayContext.Current.UserRepo.AddUser(new UserAccount
                {
                    Birthday = userDetails.Birthday,
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Password = userDetails.Password,
                    UserId = userDetails.UserId
                });

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });

            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }
        }

        [HttpPost]
        public IHttpActionResult GetUserDetails(UserModel userDetails)
        {
            try
            {

                if (userDetails == null || string.IsNullOrWhiteSpace(userDetails.UserId))
                    throw new Exception("Invalid Parameter. No UserId.");

                var result = SubwayContext.Current.UserRepo.GetUserDetails(userDetails.UserId);


                return Ok(new ResponseModel
                {
                    Status = (result == null) ? Status.Failed : Status.Success,
                    Data = result,
                    ErrorMessage = (result == null) ? "User not found." : null,
                    RecordCount = (result == null) ? 0 : 1
                });

            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDetails">Only UserId will be used here.
        /// {       
        ///    "UserId":"newUser3"
        /// }
        /// </param>
        /// <returns></returns>        
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult GetAllTalents(UserAccount userDetails)
        {
            try
            {
                if (userDetails == null || string.IsNullOrWhiteSpace(userDetails.UserId))
                    throw new Exception("Invalid Parameter. No UserId.");

                List<UserAccount> result = SubwayContext.Current.UserRepo.GetAllUsers(userDetails.UserId);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = result,
                    RecordCount = (result == null) ? 0 : result.Count
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult UpdateUser(UserModel user)
        {
            try
            {
                SubwayContext.Current.UserRepo.UpdateUser(ConvertSubwayObject.ConvertToUserAccount(user));

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }
        }
        [AuthorizationRequired]
        [HttpPost]
        public Task<ResponseModel> UpdateUserMultiPart()
        {

            if (!CheckUserId())
            {
                var task = new Task<ResponseModel>(() =>
                {
                    return new Models.ResponseModel
                    {
                        Status = Models.Status.Failed,
                        ErrorMessage = "Invalid header value. Should be UserId."
                    };
                });
                task.RunSynchronously();

                return task;
            }

            var userId = Request.Headers.GetValues("UserId").FirstOrDefault();
            var folderName = string.Format("{0}/{1}", ConfigurationManager.AppSettings["Uploads"], userId);
            var PATH = HttpContext.Current.Server.MapPath(string.Format("{0}{1}", "~/", folderName));
            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
            if (!Directory.Exists(PATH))
                Directory.CreateDirectory(PATH);

            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new SubwayMultipartFormDataStreamProvider(PATH);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<ResponseModel>(t =>
                {
                    try
                    {
                        //Todo: put logs here.
                        if (t.IsFaulted || t.IsCanceled)
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);


                        var fileInfo = streamProvider.FileData.Select(i =>
                        {
                            var fileType = string.Empty;

                            //fileType = "F";

                            var info = new FileInfo(i.LocalFileName);
                            return new FileDesc(info.Name, rootUrl + "/" + folderName + "/" + info.Name, info.Length / 1024,
                                i.Headers.ContentType.MediaType, fileType);
                        });                     

                        
                        var userModel = SaveUserDetails(userId, streamProvider, (fileInfo.Count() > 0) ? fileInfo.First().Path : string.Empty, PATH);
                                               
                         
                        AddUserFiles(fileInfo, userId,userModel.Perspective);

                        return new ResponseModel
                        {
                            Status = Status.Success,
                            Data = fileInfo
                        };

                    }
                    catch (Exception ex)
                    {
                        var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                        exceptionMessage += ". " + ex.StackTrace;

                        SubwayContext.Current.Logger.Log(exceptionMessage);

                        return new ResponseModel
                        {
                            Status = Status.Failed,
                            ErrorMessage = ex.Message
                        };
                    }
                });
                return task;

            }
            else
                throw new Exception("The request is not multi-part content.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">Only UserId will be used here.
        /// 
        ///{       
        ///  "UserId":"newUser3"
        ///}
        /// </param>
        /// <returns></returns>
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult GetUserFiles(UserAccount user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.UserId))
                    throw new Exception("Invalid Parameter. Must supply UserId.");


                var result = SubwayContext.Current.UserRepo.GetFiles(user.UserId);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = result,
                    RecordCount = (result == null) ? 0 : result.Count
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }


        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult GetPlannerCounts(UserAccount user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.UserId))
                    throw new Exception("Invalid Parameter. Must supply UserId.");


                var result = SubwayContext.Current.UserRepo.GetPlannerCounts(user.UserId);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }


        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult GetTalentCounts(UserAccount user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.UserId))
                    throw new Exception("Invalid Parameter. Must supply UserId.");


                var result = SubwayContext.Current.UserRepo.GetTalentCounts(user.UserId);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }


        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult SearchTalent(SearchModel searchModel)
        {
            try
            {
                if (searchModel == null)
                    throw new Exception("Invalid parameter.");
                if (searchModel.GenreList == null)
                    searchModel.GenreList = new List<int>();
                if (searchModel.SkillList == null)
                    searchModel.SkillList = new List<int>();

                if (searchModel.DistanceInMeters > 0)
                    if (searchModel.CurrentLocation == null && searchModel.CityStateID == null)
                        throw new Exception("Please add a your location or select a city if searching with proximity.");

                if (searchModel.Sort != null)
                {
                    if (string.IsNullOrEmpty(searchModel.Sort.Key))
                        throw new Exception("Please select a sort catagory.");
                }

                var genreList = string.Join(",", searchModel.GenreList);
                var skillList = string.Join(",", searchModel.SkillList);
                var rawResult = SubwayContext.Current.UserRepo.SearchTalent(searchModel.SearchString, genreList, skillList, searchModel.UserId);
                var usersWithLocation = LocationHelper.FilterDistace<UserAccount>(searchModel, rawResult, "Talent Search");

                //Apply Sorting
                if (searchModel.Sort != null)
                    usersWithLocation = SortHelper.Sort<UserAccount>(searchModel.Sort, usersWithLocation);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = usersWithLocation,
                    RecordCount = (usersWithLocation == null) ? 0 : usersWithLocation.Count
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }


        }
        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult DeleteUserFile(Files file)
        {
            try
            {

                if (file == null)
                    throw new Exception("Invalid Request");
                if (file.FileId == 0)
                    throw new Exception("FileId is required.");
                if (string.IsNullOrEmpty(file.UserId))
                    throw new Exception("UserId is required.");


                var folderName = ConfigurationManager.AppSettings["Uploads"];
                var PATH = HttpContext.Current.Server.MapPath("~/" + folderName + "/" + file.UserId + "/");
                var physicalFile = PATH + file.Name;

                if (!File.Exists(physicalFile))
                {
                    return Ok(new ResponseModel
                    {
                        Status = Status.Failed,
                        ErrorMessage = "File not found."
                    });
                }

                //if no return this means file exists in DB.
                var result = SubwayContext.Current.UserRepo.DeleteUserFile(string.Empty, file.FileId, string.Empty);

                if (string.IsNullOrWhiteSpace(result))
                    File.Delete(physicalFile);

                return Ok(new ResponseModel
                {
                    Status = (string.IsNullOrWhiteSpace(result)) ? Status.Success : Status.Failed,
                    ErrorMessage = (string.IsNullOrWhiteSpace(result)) ? string.Empty : result
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }


        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult SetProfilePic(SetProfilePicModel file)
        {
            try
            {

                if (file == null)
                    throw new Exception("Invalid Parameters.");
                if (string.IsNullOrWhiteSpace(file.UserId) || file.FileId == 0 || string.IsNullOrWhiteSpace(file.Perspective))
                    throw new Exception("Invalid userid or file id or perpective.");

                if (file.Perspective.ToLower() != "t" && file.Perspective.ToLower() != "p")
                    throw new Exception("Invalid perspective.");
               



                SubwayContext.Current.UserRepo.SetProfilePic(file.UserId, file.FileId,file.Perspective.ToLower());


                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }

        }
        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult ChangePassword(PasswordModel password)
        {
            try
            {

                if (password == null)
                    throw new Exception("Invalid Parameters.");
                if (string.IsNullOrWhiteSpace(password.UserId) || string.IsNullOrWhiteSpace(password.OldPassword) || string.IsNullOrWhiteSpace(password.NewPassword))
                    throw new Exception("Some of the parameters were not supplied.");



                SubwayContext.Current.UserRepo.ChangePassword(password.OldPassword, password.NewPassword, password.UserId);


                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }

        }
        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        public IHttpActionResult GetUserComments(UserAccount user)
        {
            try
            {
                if (user == null)
                    throw new Exception("Invalid parameters.");
                if (string.IsNullOrWhiteSpace(user.UserId) || string.IsNullOrWhiteSpace(user.Perspective))
                    throw new Exception("UserId and Perspective are required.");

                var result = SubwayContext.Current.UserRepo.GetUserRatingsFeedback(user.UserId, user.Perspective.ToUpper());


                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    RecordCount = result.Count,
                    Data = result
                });


            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }

        }
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult Logout(UserModel userDetails)
        {
            try
            {
                if (userDetails == null)
                    throw new Exception("Invalid parameters.");
                if (string.IsNullOrWhiteSpace(userDetails.UserId))
                    throw new Exception("UserId is required.");


                var token = Request.Headers.GetValues("Token").FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    SubwayContext.Current.TokenRepo.DeleteToken(token);
                }

                

                Task.Run(() => SubwayContext.Current.UserRepo.RemoveDeviceID(userDetails.UserId, userDetails.DeviceToken));

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }
        }
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult ChangeSetting(UserModel userDetails)
        {
            try
            {
                if (userDetails == null)
                    throw new Exception("Invalid parameters.");
                if (string.IsNullOrWhiteSpace(userDetails.UserId))
                    throw new Exception(" UserId is required.");

                SubwayContext.Current.UserRepo.ChangeSettings((userDetails.AllowNotification) ? (short)1 : (short)0, (userDetails.AllowEmail) ? (short)1 : (short)0, userDetails.UserId);

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = exceptionMessage
                });
            }
        }
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult GetUserNotifications(UserModel userDetails)
        {
            try
            {
                if (userDetails == null)
                    throw new Exception("Invalid parameters.");
                if (string.IsNullOrWhiteSpace(userDetails.UserId))
                    throw new Exception(" UserId is required.");

                var result = SubwayContext.Current.UserRepo.GetUserNotification(userDetails.UserId);

                return Ok(new ResponseModel
                {
                    Status = Status.Success,
                    Data = result,
                    RecordCount = result.Count
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }
        [AuthorizationRequired]
        [HttpPost]
        public IHttpActionResult DeleteUserNotification(LookUpValues details)
        {
            try
            {
                if (details == null)
                    throw new Exception("Invalid parameters.");
                if (details.Id == 0)
                    throw new Exception("Id is required.");

                SubwayContext.Current.UserRepo.DeleteUserNotification(details.Id);

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        public IHttpActionResult ForgotPassword(UserModel details)
        {
            try
            {
                if (details == null)
                    throw new Exception("Invalid parameters.");
                if (string.IsNullOrWhiteSpace(details.UserId))
                    throw new Exception("UserId is required.");

                var userDetails = SubwayContext.Current.UserRepo.GetUserDetails(details.UserId);

                if (userDetails == null)
                    throw new Exception("User not found.");


                var generatedPassword = System.Web.Security.Membership.GeneratePassword(10, 3);
                SubwayContext.Current.UserRepo.UpdatePassword(userDetails.UserId, generatedPassword);

                var emailBody = EmailContentFactory.GetForgotPasswordTemplate(userDetails.FirstName + " " + userDetails.LastName, generatedPassword);

                EmailSender.SendMail("", userDetails.Email, emailBody, ConfigurationManager.AppSettings["ForgotPassword.Subject"]);

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        public IHttpActionResult SendHelp(HelpModel details)
        {
            try
            {
                if (details == null)
                    throw new Exception("Invalid parameters.");
                if (string.IsNullOrWhiteSpace(details.SenderEmail) || string.IsNullOrWhiteSpace(details.SenderName) ||
                    string.IsNullOrWhiteSpace(details.Subject) || string.IsNullOrWhiteSpace(details.Message))
                    throw new Exception("Email, Name, Subject and Message are required.");


                SubwayContext.Current.UserRepo.AddHelp(details.UserId, details.SenderName, details.SenderEmail, details.Subject, details.Message);

                var emailBody = EmailContentFactory.GetHelpTemplate(details.SenderName);
                var emailToAdminBody = EmailContentFactory.GetHelpToAdminTemplate(details.SenderName, details.SenderEmail, details.Message);

                EmailSender.SendMail("", details.SenderEmail, emailBody, ConfigurationManager.AppSettings["Help.Subject"]);
                EmailSender.SendMail("", ConfigurationManager.AppSettings["Help.Email"], emailToAdminBody, ConfigurationManager.AppSettings["Help.AdminSubject"]);

                return Ok(new ResponseModel
                {
                    Status = Status.Success
                });
            }
            catch (Exception ex)
            {
                var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                exceptionMessage += ". " + ex.StackTrace;

                SubwayContext.Current.Logger.Log(exceptionMessage);
                return Ok(new ResponseModel
                {
                    Status = Status.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        #region "Private Method(s)"

        private UserModel SaveUserDetails(string userId, SubwayMultipartFormDataStreamProvider streamProvider, string profilePicPath, string phyicalPath)
        {
            var userModel = PopulateUserModelProperties(streamProvider, userId);

            //if there is a new profile pic
            if (!string.IsNullOrWhiteSpace(profilePicPath))
            {

                //delete the record first if planner perspective 
                //if talent just retain and update the filetype to regular pic.
                if (userModel.Perspective.ToLower() == "p")
                {
                    var physicalFile = string.Format("{0}/{1}", phyicalPath, Path.GetFileName(userModel.ProfilePic));

                    SubwayContext.Current.UserRepo.DeleteUserFile(userId, 0, Path.GetFileName(userModel.ProfilePic));
                    if (File.Exists(physicalFile))
                        File.Delete(physicalFile);
                    
                    userModel.ProfilePic = profilePicPath;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(userModel.ProfilePicTalent))
                        SubwayContext.Current.UserRepo.UpdateUserFile(userId, Path.GetFileName(userModel.ProfilePicTalent), "P");
                    userModel.ProfilePicTalent = profilePicPath;
                }
            }
            userModel.UserId = userId;
            SubwayContext.Current.UserRepo.UpdateUser(ConvertSubwayObject.ConvertToUserAccount(userModel));
            return userModel;

        }

        private bool CheckUserId()
        {
            var userIdheader = Request.Headers.FirstOrDefault(h =>
            {
                return h.Key == "UserId";
            });

            return (userIdheader.Key == null) ? false : true;
        }

        private void AddUserFiles(IEnumerable<FileDesc> fileInfo, string userId, string perspective)
        {
            foreach (var file in fileInfo)
            {
                //if planner updates profile pic
                if (perspective.ToLower() == "p")
                    file.FileType = "PF";
                //if talent updates profile pic
                if (perspective.ToLower() == "t")
                    file.FileType = "TF";
                SubwayContext.Current.UserRepo.AddFile(userId, file.FileType, file.Path, file.Name, "");
            }

        }

        private UserModel PopulateUserModelProperties(SubwayMultipartFormDataStreamProvider streamProvider, string userId)
        {
            var userModel = new UserModel();
            userModel.Skills = new List<Skills>();
            userModel.Genres = new List<Genre>();
            userModel.SoundCloud = new List<ExternalMedia>();
            userModel.Youtube = new List<ExternalMedia>();


            foreach (var data in streamProvider.FormData)
            {
                PropertyInfo prop = userModel.GetType().GetProperty(data.ToString(), System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                switch (data.ToString())
                {
                    case "Skills[]":
                        {
                            var skills = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var skill in skills)
                            {
                                userModel.Skills.Add(new Skills
                                {
                                    Id = Convert.ToInt32(skill)
                                });
                            }
                            break;
                        }
                    case "Genre[]":
                        {
                            var genres = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var genre in genres)
                            {
                                userModel.Genres.Add(new Genre
                                {
                                    Id = Convert.ToInt32(genre)
                                });
                            }
                            break;
                        }
                    case "SoundCloud[][Id]":
                        {
                            var soundId = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var sound in soundId)
                            {
                                userModel.SoundCloud.Add(new ExternalMedia
                                {
                                    Id = Convert.ToInt32(sound)
                                });
                            }
                            break;

                        }
                    case "SoundCloud[][Name]":
                        {
                            var soundNames = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (var i = 0; i < soundNames.Count(); i++)
                            {
                                userModel.SoundCloud[i].Name = soundNames[i];
                            }
                            break;

                        }
                    case "SoundCloud[][ThumbnailUrl]":
                        {
                            var soundNames = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (var i = 0; i < soundNames.Count(); i++)
                            {
                                userModel.SoundCloud[i].ThumbnailUrl = soundNames[i];
                            }
                            break;

                        }
                    case "SoundCloud[][Url]":
                        {
                            var soundNames = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (var i = 0; i < soundNames.Count(); i++)
                            {
                                userModel.SoundCloud[i].Url = soundNames[i];
                            }
                            break;

                        }
                    case "SoundCloud[][ExternalType]":
                        {

                            break;

                        }
                    case "Youtube[][Id]":
                        {
                            var soundId = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var sound in soundId)
                            {
                                userModel.Youtube.Add(new ExternalMedia
                                {
                                    Id = Convert.ToInt32(sound)
                                });
                            }
                            break;

                        }
                    case "Youtube[][Name]":
                        {
                            var youtubeNames = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (var i = 0; i < youtubeNames.Count(); i++)
                            {
                                userModel.Youtube[i].Name = youtubeNames[i];
                            }
                            break;

                        }
                    case "Youtube[][Url]":
                        {
                            var youtubeNames = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (var i = 0; i < youtubeNames.Count(); i++)
                            {
                                userModel.Youtube[i].Url = youtubeNames[i];
                            }
                            break;

                        }
                    case "Youtube[][ThumbnailUrl]":
                        {
                            var youtubeNames = streamProvider.FormData[data.ToString()].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            for (var i = 0; i < youtubeNames.Count(); i++)
                            {
                                userModel.Youtube[i].ThumbnailUrl = youtubeNames[i];
                            }
                            break;

                        }
                    case "Youtube[][ExternalType]":
                        {

                            break;
                        }

                    default:
                        {
                            try
                            {
                                if (streamProvider.FormData[data.ToString()] == null)
                                {
                                    SubwayContext.Current.Logger.Log(string.Format("Warning : Invalid multipart key [{0}]. UpdateUser for [{1}]", data.ToString(), userId));
                                }
                                else
                                    prop.SetValue(userModel, ConvertToProperType(prop.PropertyType.Name, streamProvider.FormData[data.ToString()]));

                            }
                            catch (Exception ex)
                            {
                                SubwayContext.Current.Logger.Log(string.Format("Error : Invalid multipart key [{0}]. UpdateUser for [{1}] or something went wrong with the request.", data.ToString(), userId));
                                throw ex;
                            }
                            break;
                        }
                }
            }

            return userModel;
        }

        private dynamic ConvertToProperType(string propertyType, string val)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Int32.Parse(val);
                case "DateTime":
                    return DateTime.Parse(val);
                case "Single":
                    return Single.Parse(val);
                default:
                    return val;
            }
        }

        private List<string> ValidateLogin(UserModel user)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(user.UserId))
                errors.Add("userid is null");
            if (string.IsNullOrWhiteSpace(user.Password))
                errors.Add("password is null");
            //if (user.Device == 0)
            //    errors.Add("Device is required.");
            //else
            //    if (string.IsNullOrWhiteSpace(user.DeviceToken))
            //        errors.Add("DeviceToken is required. ");


            return errors;
        }

        private List<string> ValidateNewUser(UserModel user)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(user.UserId))
                errors.Add("userid is null");
            if (string.IsNullOrWhiteSpace(user.Email))
                errors.Add("email is null");
            if (string.IsNullOrWhiteSpace(user.FirstName))
                errors.Add("email is firstname");
            if (string.IsNullOrWhiteSpace(user.LastName))
                errors.Add("email is lastname");


            return errors;
        }

        private void AddFBUser(UserModel userDetails)
        {
            if (userDetails.Device == 0)
                throw new Exception("Device is required.");
            else
                if (string.IsNullOrWhiteSpace(userDetails.DeviceToken))
                    throw new Exception("DeviceToken is required. ");

            var userobj = SubwayContext.Current.UserRepo.GetUserDetails(userDetails.UserId);

            if (userobj == null)
            {
                var DtoUserDetails = new UserAccount
                {
                    Birthday = userDetails.Birthday,
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    UserId = userDetails.UserId,
                    FacebookUser = true,
                    LastLoggedInDate = DateTime.UtcNow
                };

                SubwayContext.Current.UserRepo.AddUser(DtoUserDetails);
            }

            SubwayContext.Current.UserRepo.AddDeviceID(userDetails.UserId, userDetails.DeviceToken, userDetails.Device);
        }

        #endregion

    }
}
