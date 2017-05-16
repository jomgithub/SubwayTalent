using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts.Interfaces
{
    public interface ISettings
    {
        void AddSettings(string key, string value);

        void UpdateSettings(string key, string value);

        void DeleteSettings(string key);

       Dictionary<string,string> GetSettings();

       string GetSetting(string key);
    }
}
