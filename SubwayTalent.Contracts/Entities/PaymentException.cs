using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts.Entities
{
    public enum PaymentEvent
    { 
        AddPaymentMethod,
        CreatePayment,
        CapturePayment
    }

    public class PaymentException
    {
        public int EventPlannerId { get; set; }
        public int EventInviteId { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public PaymentEvent PaymentEventName { get; set; }
        public string OtherInfo { get; set; }
        public string UserId { get; set; }
    }
}
