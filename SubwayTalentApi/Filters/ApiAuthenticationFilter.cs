using SubwayTalentApi.Filters;
using SubwayTalentApi.Helpers;
using SubwayTalentApi.Models;
using System;
using System.Threading;
using System.Web.Http.Controllers;

namespace SubwayTalentApi.Filters
{
    /// <summary>
    /// Custom Authentication Filter Extending basic Authentication
    /// </summary>
    public class ApiAuthenticationFilter : GenericAuthenticationFilter
    {
        /// <summary>
        /// Default Authentication Constructor
        /// </summary>
        public ApiAuthenticationFilter()
        {
        }

        /// <summary>
        /// AuthenticationFilter constructor with isActive parameter
        /// </summary>
        /// <param name="isActive"></param>
        public ApiAuthenticationFilter(bool isActive)
            : base(isActive)
        {
        }

        /// <summary>
        /// Protected overriden method for authorizing user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool OnAuthorizeUser(string username, string password, string accessToken)
        {
            var userModel = new UserModel
            {
                 UserId = username,
                 Password = password,
                 AccessToken = accessToken
            };

            var userObj = LoginHelper.AuthenticateUser(userModel);

            if (userObj != null)
            {
                var basicAuthenticationIdentity = Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                {
                    basicAuthenticationIdentity.UserId = userObj.UserId;
                    basicAuthenticationIdentity.AccessToken = userObj.AccessToken;
                    basicAuthenticationIdentity.FirstName = userObj.FirstName;
                    basicAuthenticationIdentity.LastName = userObj.LastName;
                    basicAuthenticationIdentity.Email = userObj.Email;
                    basicAuthenticationIdentity.Birthday = userObj.Birthday;  
                                     
                }
                    
                return true;
            }
               
            return false;
        }
    }
}