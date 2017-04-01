using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubwayTalent.Core;
using System.Configuration;
using MoonAPNS;

namespace SubwayTalent.Core.Providers
{
    public class APNProvider : IAPNPushNotification
    {
        public void Send(SubwayNotificationPayload payload)
        {
           
            CheckSettings();
            var iosCert = ConfigurationManager.AppSettings["Apple.CertLocation.Sandbox"];
            var iosCertPassword = ConfigurationManager.AppSettings["Apple.CertPassword"];

            if (payload == null)
                throw new Exception("payload is null.");

            if (payload.Device != DeviceType.Apple)
                throw new Exception("payload is not for IOS");

            //check if sandbox or production cert.
            var isProd = (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Apple.IsProd"]) ? false : Convert.ToBoolean(ConfigurationManager.AppSettings["Apple.IsProd"]));

            if (isProd)
                iosCert = ConfigurationManager.AppSettings["Apple.CertLocation"];            
                    
            var iosPayload = new NotificationPayload(payload.DeviceToken, payload.AlertMessage, payload.Badge, "default");

            if (payload.CustomItems != null && payload.CustomItems.Count > 0)
            {
                foreach (var item in payload.CustomItems)
                {
                    iosPayload.AddCustom(item.Key, item.Value);
                }
            }

            var push = new PushNotification(!isProd, iosCert, iosCertPassword);

            var result = push.SendToApple(new List<NotificationPayload> { iosPayload });
           
        }

        private void CheckSettings()
        {
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Apple.CertLocation"]))
                throw new Exception("Missing Certificate for Apple Push Notifacation in config file [Apple.CertLocation]");
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Apple.CertLocation.Sandbox"]))
                throw new Exception("Missing Certificate for Apple Push Notifacation in config file [Apple.CertLocation.Sandbox]");

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Apple.IsProd"]))
                throw new Exception("Missing Certificate for Apple Push Notifacation in config file [Apple.IsProd]");
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Apple.CertPassword"]))
                throw new Exception("Missing Certificate for Apple Push Notifacation in config file [Apple.CertPassword]");

        }


      
    }
 

}
