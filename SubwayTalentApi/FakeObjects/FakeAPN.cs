using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalentApi.FakeObjects
{
    public class FakeAPN : IAPNPushNotification
    {
        public void Send(SubwayTalent.Core.SubwayNotificationPayload payload)
        {
            SubwayContext.Current.Logger.Log("<FakeAPN>");
            
            SubwayContext.Current.Logger.Log(string.Format("alertMessage:{0}, badge:{1}, device:{2}, deviceToken:{3} ", 
                                        payload.AlertMessage, payload.Badge.ToString(),payload.Device.ToString(),
                                        payload.DeviceToken));
            payload.CustomItems.ToList().ForEach(x =>
                {
                    SubwayContext.Current.Logger.Log(string.Format("Key:{0}, value:{1}", x.Key, x.Value[0]));
                });
            SubwayContext.Current.Logger.Log("</FakeAPN>");
        }
    }
}