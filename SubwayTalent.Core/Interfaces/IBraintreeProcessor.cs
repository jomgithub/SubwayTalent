using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Interfaces
{
    public interface IBraintreeProcessor
    {
        /// <summary>
        /// Returns a customer id.
        /// 
        /// </summary>
        /// <param name="paymentNonce">this is from client. Repsentation of a payment mode.</param>
        /// <returns></returns>
        Dictionary<string,string> CreateCustomer(string paymentNonce, string firstName, string lastName, string email);

        
    }
}
