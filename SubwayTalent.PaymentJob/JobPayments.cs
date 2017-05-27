using Newtonsoft.Json;
using SubwayTalent.Contracts.Entities;
using SubwayTalent.Core.Interfaces;
using SubwayTalent.Core.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.PaymentJob
{
    public class JobPayments
    {

        private IPaymentProcessor payProcessor = null;
        private string transAuthId = string.Empty;
        private float totalTalentPayments;
        private float adminPayPercent;
        private float totalPlannerPayments;
        private float handlingFee;
        private string processor = "SubwayTalent.Core.Processors.{0}, SubwayTalent.Core";

        public void ProcessFuturePayments()
        {
            var results = SubwayContext.Current.EventRepo.GetDoneEventsPaymentInfo();
            SubwayContext.Current.Logger.Log("Done Events Count " + results.Count.ToString());

            foreach (var plannerPayment in results)
            {
                try
                {
                    //pending
                    if (plannerPayment.PaymentStatus == 0)
                    {
                        SubwayContext.Current.Logger.Log("Process Future payments started.");

                        var eventDetails = SubwayContext.Current.EventRepo.GetEventDetails(plannerPayment.EventId);
                        SubwayContext.Current.Logger.Log("event name : " + eventDetails.Name);
                        SubwayContext.Current.Logger.Log("Talent count : " + eventDetails.Talents.Count.ToString());

                        totalTalentPayments = 0;

                        eventDetails.Talents.ForEach(t =>
                            {
                                //only add those talents that are confirmed
                                if (t.Status == 1)
                                    totalTalentPayments += t.EventRate;

                            });

                        if (totalTalentPayments <= 0)
                        {
                            SubwayContext.Current.PaymentRepo.UpdatePlannerPaymentStatus(plannerPayment.EventPlannerId, 4);
                            SubwayContext.Current.Logger.Log(string.Format("No confirmed talents or zero amount to be created for event[{0}]", plannerPayment.EventId.ToString()));
                            continue;
                        }

                        var processorType = string.Format(processor, plannerPayment.EventPayment.Method.Processor);

                        payProcessor = (IPaymentProcessor)Activator.CreateInstance(System.Type.GetType(processorType, true), SubwayContext.Current.SubwaySettings);

                        transAuthId = payProcessor.CreatePayment(totalTalentPayments, plannerPayment.EventPayment.RefreshToken);

                        //Save transacation authorization id
                        SubwayContext.Current.PaymentRepo.AddTransactionIds(transAuthId, null, plannerPayment.EventPlannerId);
                        SubwayContext.Current.PaymentRepo.UpdatePlannerPaymentStatus(plannerPayment.EventPlannerId, 2);

                        //update the payment status of the talents to 2-created
                        SubwayContext.Current.PaymentRepo.UpdateTalentPaymentStatus(plannerPayment.EventPlannerId, 2);

                        SubwayContext.Current.Logger.Log("Successul Payment Creation for event[" + plannerPayment.EventId + "] with transID [" + transAuthId + "]");

                    }

                }
                catch (Exception ex)
                {
                    var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                    exceptionMessage += ". " + ex.StackTrace;

                    var exId = SubwayContext.Current.PaymentRepo.AddPaymentException(new PaymentException
                    {
                        StackTrace = ex.StackTrace + ". " + ((ex.InnerException != null) ? ex.InnerException.StackTrace : string.Empty),
                        PaymentEventName = PaymentEvent.CreatePayment,
                        Message = ex.Message,
                        EventPlannerId = plannerPayment.EventPlannerId,
                        UserId = plannerPayment.EventPayment.UserId,
                        OtherInfo = JsonConvert.SerializeObject(plannerPayment)
                    });

                    SubwayContext.Current.PaymentRepo.UpdatePlannerPaymentStatus(plannerPayment.EventPlannerId, 3);
                    SubwayContext.Current.Logger.Log(string.Format("exception ID : [{0}], {1}", exId, exceptionMessage));
                }

            }
        }



        public void ProcessPayments()
        {

            SubwayContext.Current.Logger.Log("ProcessPayments started.");
            var results = SubwayContext.Current.EventRepo.GetDoneEventsPaymentInfo();

            foreach (var plannerPayment in results)
            {
                try
                {
                    var transactionIdCompleted = string.Empty;
                    //filter all created payments
                    if (plannerPayment.PaymentStatus == 2)
                    {
                        var eventDetails = SubwayContext.Current.EventRepo.GetEventDetails(plannerPayment.EventId);
                        totalTalentPayments = 0;

                        eventDetails.Talents.ForEach(t =>
                            {
                                //only add those talents that are confirmed
                                if (t.Status == 1)
                                    totalTalentPayments += t.EventRate;

                            });

                        if (totalTalentPayments <= 0)
                        {
                            SubwayContext.Current.PaymentRepo.UpdatePlannerPaymentStatus(plannerPayment.EventPlannerId, 4);
                            SubwayContext.Current.Logger.Log(string.Format("No confirmed talents or zero amount to be paid for event[{0}]", plannerPayment.EventId.ToString()));
                            continue;
                        }


                        var processorType = string.Format(processor, plannerPayment.EventPayment.Method.Processor);

                        payProcessor = (IPaymentProcessor)Activator.CreateInstance(System.Type.GetType(processorType, true), SubwayContext.Current.SubwaySettings);
                        transactionIdCompleted = payProcessor.CapturePayment(totalTalentPayments, plannerPayment.TransactionAuthId, plannerPayment.EventPayment.RefreshToken);

                        //Save transacation authorization id
                        SubwayContext.Current.PaymentRepo.AddTransactionIds(plannerPayment.TransactionAuthId, transactionIdCompleted, plannerPayment.EventPlannerId);

                        SubwayContext.Current.PaymentRepo.UpdatePlannerPaymentStatus(plannerPayment.EventPlannerId, 1);

                        //update the payment status of the talents to 1-paid
                        SubwayContext.Current.PaymentRepo.UpdateTalentPaymentStatus(plannerPayment.EventPlannerId, 1);

                        SubwayContext.Current.Logger.Log("Successul Payment for event[" + plannerPayment.EventId + "] with transID [" + transactionIdCompleted + "]");

                    }
                }
                catch (Exception ex)
                {
                    var exceptionMessage = (ex.InnerException == null) ? ex.Message : ex.Message + ". " + ex.InnerException.Message;
                    exceptionMessage += ". " + ex.StackTrace;

                    var exId = SubwayContext.Current.PaymentRepo.AddPaymentException(new PaymentException
                    {
                        StackTrace = ex.StackTrace + ". " + ((ex.InnerException != null) ? ex.InnerException.StackTrace : string.Empty),
                        PaymentEventName = PaymentEvent.CapturePayment,
                        Message = ex.Message,
                        EventPlannerId = plannerPayment.EventPlannerId,
                        UserId = plannerPayment.EventPayment.UserId,
                        OtherInfo = JsonConvert.SerializeObject(plannerPayment)
                    });
                    SubwayContext.Current.PaymentRepo.UpdatePlannerPaymentStatus(plannerPayment.EventPlannerId, 3);
                    SubwayContext.Current.Logger.Log(string.Format("exception ID : [{0}], {1}", exId, exceptionMessage));
                }
            }
        }
    }
}
