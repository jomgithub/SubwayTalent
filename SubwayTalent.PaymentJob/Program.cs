using SubwayTalent.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SubwayTalent.PaymentJob
{
    class Program
    {
        private static JobPayments _job = null;
        private static Timer _timer;
        private static Timer _timerCapture;
        private static int paymentCreationInterval = int.Parse(ConfigurationManager.AppSettings["paymentCreationInterval"]);
        private static int paymentProcessInterval = int.Parse(ConfigurationManager.AppSettings["paymentProcessInterval"]);
        static void Main(string[] args)
        {

            _job = new JobPayments();
            SubwayContext.Current.Logger.Log("Payment Processor Started.....");

            //every 5mins
            _timer = new Timer(PaymentCreationJob, null, 0, paymentCreationInterval);

            //every 1min
            _timerCapture = new Timer(PaymentCaptureJob, null, 0, paymentProcessInterval);

            Console.ReadLine();
        }

        private static void PaymentCaptureJob(object state)
        {
            var currentTime = DateTime.Now.ToString("HH:mm");
            var captureTime = ConfigurationManager.AppSettings["CaptureTime"];
            bool isDebug = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["paymentDebug"]) ? false : bool.Parse(ConfigurationManager.AppSettings["paymentDebug"]);
            if ((currentTime == captureTime) || isDebug)
            {
                _job.ProcessPayments();
            }
        }

        private static void PaymentCreationJob(object state)
        {
            SubwayContext.Current.Logger.Log("Payment Creation checking.....");
            _job.ProcessFuturePayments();
        }
    }
}
