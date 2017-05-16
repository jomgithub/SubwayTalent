using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts
{
    public class EventPlannerPayment
    {
        public int EventPlannerId { get; set; }
        public int EventId { get; set; }
        //0-pending,1-paid,2-created,3-exeptions(problems in payment),4(done event but no talents to be paid)
        public short PaymentStatus { get; set; }
        public string TransactionAuthId { get; set; }
        public string TransactionIdCompleted { get; set; }
        public Payment EventPayment { get; set; }
    }
}
