using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core
{
    public class ProviderFactory
    {
        public static T CreateProvider<T>(string concreteType = "") where T : class
        {
            string interfaceName = typeof(T).Name;

            // get the name of the type to instantiate
            string typeName = string.IsNullOrWhiteSpace(concreteType) ? ConfigurationManager.AppSettings[interfaceName]  : concreteType;
            if (string.IsNullOrEmpty(typeName))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Specified interface type {0} is not registered in the configuration file.", interfaceName));

            // instantiate the type, throwing an error if it's not found
            return (T)Activator.CreateInstance(Type.GetType(typeName, true));
        }


    }
}
