using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Contracts.Interfaces
{
    public enum LookUpValueType
    {
        Skills = 0,
        Genres = 1,
        EventTypes
    }

    public enum UserTypes
    {
        Talent = 0,
        Planner = 1
    }

    public enum ExternalMediaType
    {
        SoundCloud = 0,
        Youtube = 1
    }

    public interface ILookUpValues
    {
        IList<LookUpValues> GetAllValues(LookUpValueType lookUpType);
        void AddLookUpValue(LookUpValues lookupValue, LookUpValueType lookUpType);
        void DeleteLookUpValue(LookUpValues lookupValue, LookUpValueType lookUpType);
        void UpdateLookUpValue(LookUpValues lookupValue, LookUpValueType lookUpType);
        IList<States> GetAllStates();
        IList<City> GetCityByStateId(string stateId);
        IList<City> GetCityInfoByCityStateId(string cityStateIds);
    }
}
