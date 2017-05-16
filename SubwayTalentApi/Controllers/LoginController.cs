using SubwayTalent.Contracts;
using SubwayTalentApi.Filters;
using SubwayTalentApi.Models;
using SubwayTalentApi.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SubwayTalentApi.Controllers
{
    [ApiAuthenticationFilter]
    public class LoginController : ApiController
    {

        #region Private variable.

        private readonly ITokenServices _tokenServices;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public LoginController(ITokenServices tokenServices)
        {
            _tokenServices = tokenServices;
        }

        public LoginController()
        {
            _tokenServices = new TokenServices();
        }

        #endregion

        /// <summary>
        /// Authenticates user and returns token with expiry.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Authenticate(UserModel user)
        {
            if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                {


                    user.UserId = basicAuthenticationIdentity.UserId;
                    user.Birthday = basicAuthenticationIdentity.Birthday;
                    user.FirstName = basicAuthenticationIdentity.FirstName;
                    user.LastName = basicAuthenticationIdentity.LastName;
                    user.AccessToken = basicAuthenticationIdentity.AccessToken;

                    //for fb user that doesn't exists in DB               
                    if (SubwayContext.Current.UserRepo.GetUserDetails(basicAuthenticationIdentity.UserId) == null)
                    {
                        var DtoUserDetails = new UserAccount
                        {
                            Birthday = basicAuthenticationIdentity.Birthday,
                            Email = basicAuthenticationIdentity.Email,
                            FirstName = basicAuthenticationIdentity.FirstName,
                            LastName = basicAuthenticationIdentity.LastName,
                            UserId = basicAuthenticationIdentity.UserId,
                            FacebookUser = true,
                            LastLoggedInDate = DateTime.UtcNow
                        };

                        SubwayContext.Current.UserRepo.AddUser(DtoUserDetails);
                    }

                    //Add deviceId token for push notifications.
                    if (user.Device != 0 && !string.IsNullOrWhiteSpace(user.DeviceToken))
                        SubwayContext.Current.UserRepo.AddDeviceID(basicAuthenticationIdentity.UserId, user.DeviceToken, user.Device);


                    return ResponseMessage(GetAuthToken(user));

                  
                }
            }
            return null;
        }

        /// <summary>
        /// Returns auth token for the validated user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private HttpResponseMessage GetAuthToken(UserModel userModel)
        {
            var token = _tokenServices.GenerateToken(userModel.UserId);


            var response = Request.CreateResponse(HttpStatusCode.OK, new ResponseModel
                {
                    Status = Models.Status.Success,
                    Data = new LoginModel
                    {
                        User = userModel,
                        Token = token.AuthToken,
                        TokenExpiry = ConfigurationManager.AppSettings["AuthTokenExpiry"]
                    }
                });

            response.Headers.Add("Token", token.AuthToken);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["AuthTokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
            return response;
        }

    }
}
