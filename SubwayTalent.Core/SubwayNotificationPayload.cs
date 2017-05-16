using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core
{
    public enum DeviceType : byte
    {
        Apple = 1,
        Android = 2
    }

    public class SubwayNotificationPayload
    {
        public SubwayNotificationPayload()
        {
            this.CustomItems = new Dictionary<string, object[]>();
        }

        public DeviceType Device { get; set; }
        public string DeviceToken { get; set; }
        public string AlertMessage { get; set; }
        public int Badge { get; set; }
        public Dictionary<string, object[]> CustomItems { get; private set; }

        public void AddCustom(string key, params object[] values)
        {
            this.CustomItems.Add(key, values);
        }
    }
}
