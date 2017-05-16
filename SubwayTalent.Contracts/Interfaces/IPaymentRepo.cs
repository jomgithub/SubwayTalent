using SubwayTalent.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts.Interfaces
{
    public interface IPaymentRepo
    {
        List<PaymentMethod> GetPaymentMethods();
        void AddTransactionIds(string transactionAuthId, string transactionIdCompleted, int eventPlannerId);
        void UpdateTalentPaymentStatus(int eventPlannerId, short statusId);
        void UpdatePlannerPaymentStatus(int eventPlannerId, short statusId);
        int AddPaymentException(PaymentException paymentInfo);
    }
}
