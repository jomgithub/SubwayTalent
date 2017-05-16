using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Interfaces
{
    public interface IPaymentProcessor
    {
       
        /// <summary>
        /// Creates a payment for future use.
        /// </summary>
        /// <param name="total"></param>       
        /// <param name="token"></param>
        /// <returns>transaction's authorizationId</returns>
        string CreatePayment(float total, string token);

        /// <summary>
        ///  Executes the payment
        /// </summary>
        /// <param name="total"></param>
        /// <param name="authorizationId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        string CapturePayment(float total, string authorizationId, string token);

        /// <summary>
        /// authorizes a payment method depending on the payment gateway.
        /// </summary>
        /// <param name="authorizationCode">token to authorize a payment.</param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        IDictionary<string,string> CreatePaymetMethod(string authorizationCode, string firstName, string lastName, string email);
    }
}
