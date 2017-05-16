using SubwayTalent.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubwayTalent.Contracts
{
    public class Payment
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
        public string PaymentInstrumentId { get; set; }
        public string MaskedCardNumber { get; set; }
        public string CardType { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
