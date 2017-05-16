using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts.Interfaces
{
    public interface ITokenRepo
    {

        void AddToken(Token token);

        Token GetToken(string token, DateTime expireReference);

        void DeleteToken(string token);

        void UpdateToken(Token token);

        void DeleteTokenByUserId(string userId);
    }
}
