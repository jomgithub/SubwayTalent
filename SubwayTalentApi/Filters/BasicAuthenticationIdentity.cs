using System;
using System.Security.Principal;

namespace SubwayTalentApi.Filters
{
    /// <summary>
    /// Basic Authentication identity
    /// </summary>
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        /// <summary>
        /// Get/Set for password
        /// </summary>
        public string Password { get; set; }
       
        /// <summary>
        /// Get/Set for UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Get/Set for AccessToken. For FB login
        /// </summary>
        public string AccessToken { get; set; }


        /// <summary>
        /// Get/Set for Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Get/Set for FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get/Set for Birthday
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Get/Set for LastName
        /// </summary>
        public string LastName { get; set; }

        public BasicAuthenticationIdentity(string userId, string password, string accessToken)
            : base(userId, "SubwayTalentAuth")
        {
            Password = password;
            UserId = userId;
            AccessToken = accessToken;
        }

        public BasicAuthenticationIdentity(string userId, string password, string accessToken, string email, string firstName,
                                            string lastName, DateTime birthDay)
            : base(userId, "SubwayTalentAuth")
        {
            Password = password;
            UserId = userId;
            AccessToken = accessToken;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Birthday = birthDay;
        }
    }
}