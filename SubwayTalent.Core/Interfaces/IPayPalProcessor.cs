using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Interfaces
{
    public interface IPayPalProcessor
    {
        /// <summary>
        /// Return a refresh token that can be used when getting an access token.
        /// 
        /// </summary>
        /// <param name="authorizationCode">This is very short lived code. </param>
        /// <returns></returns>
        string CreateRefreshToken(string authorizationCode);

        /// <summary>
        /// Returns an access token than can be used in making payments.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        string CreateAccessToken(string refreshToken);
    }
}
