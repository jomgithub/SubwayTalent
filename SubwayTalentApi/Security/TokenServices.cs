using System;
using System.Configuration;
using System.Linq;
using SubwayTalent.Contracts;
using SubwayTalentApi.Models;
using SubwayTalentApi.Filters;

namespace SubwayTalentApi.Security
{
    public class TokenServices : ITokenServices
    {
        #region Private member variables.


        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public TokenServices()
        {

        }
        #endregion


        #region Public member methods.

        /// <summary>
        ///  Function to generate unique token with expiry against the provided userId.
        ///  Also add a record in database for generated token.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TokenEntity GenerateToken(string userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime issuedOn = DateTime.UtcNow;
            DateTime expiredOn = DateTime.UtcNow.AddSeconds(
                                              Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
            var tokendomain = new Token
                                  {
                                      UserId = userId,
                                      AuthToken = token,
                                      IssuedOn = issuedOn,
                                      ExpiresOn = expiredOn
                                  };

            SubwayContext.Current.TokenRepo.AddToken(tokendomain);

            var tokenModel = new TokenEntity()
                                 {
                                     UserId = userId,
                                     IssuedOn = issuedOn,
                                     ExpiresOn = expiredOn,
                                     AuthToken = token
                                 };

            return tokenModel;
        }

        /// <summary>
        /// Method to validate token against expiry and existence in database.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateToken(string token)
        {

            var tokenEntity = SubwayContext.Current.TokenRepo.GetToken(token, DateTime.UtcNow);

            if (tokenEntity == null)
                return false;

            //Expired token. Kill it
            if (tokenEntity.ExpiresOn < DateTime.UtcNow)
            {
                Kill(token);
                return false;
            }

            var elapsedSeconds = tokenEntity.ExpiresOn.Subtract(DateTime.UtcNow).TotalSeconds;
            var newExpiresOn = Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]) - elapsedSeconds;
            tokenEntity.ExpiresOn = tokenEntity.ExpiresOn.AddSeconds(newExpiresOn);
            SubwayContext.Current.TokenRepo.UpdateToken(tokenEntity);
            return true;

        }

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="token">true for successful delete</param>
        public bool Kill(string token)
        {
            SubwayContext.Current.TokenRepo.DeleteToken(token);
            return true;
        }

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true for successful delete</returns>
        public bool DeleteByUserId(string userId)
        {
            SubwayContext.Current.TokenRepo.DeleteTokenByUserId(userId);
            return true;
        }

        #endregion
    }
}
