using SubwayTalent.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubwayTalent.Core;

namespace SubwayTalent.Core.Interfaces
{
    public interface IAPNPushNotification
    {

        void Send(SubwayNotificationPayload payload);

    }
}
