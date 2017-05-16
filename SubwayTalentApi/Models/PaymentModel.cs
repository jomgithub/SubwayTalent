using SubwayTalent.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubwayTalentApi.Models
{
    public class PaymentModel
    {
        public string UserId { get; set; }
        /// <summary>
        /// payment representation
        /// </summary>
        public string AuthorizationCode { get; set; }
        /// <summary>
        /// 1-paypal, 2-creditcard/braintree
        /// </summary>
        public Int16 PaymentMethodId { get; set; }
        
    }
}
