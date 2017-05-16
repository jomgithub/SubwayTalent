using Braintree;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubwayTalent.Core.Processors
{
    public static class BraintreeConfig
    {
        private static BraintreeGateway config = null;



        public static BraintreeGateway Gateway()
        {
            if (config == null)
            {
                CheckSettings();

                config = new BraintreeGateway
                {
                    Environment = Braintree.Environment.ParseEnvironment(ConfigurationManager.AppSettings["Braintree.mode"]),
                    MerchantId = ConfigurationManager.AppSettings["Braintree.merchantId"],
                    PublicKey = ConfigurationManager.AppSettings["Braintree.publicKey"],
                    PrivateKey = ConfigurationManager.AppSettings["Braintree.privateKey"]
                };
            }

            return config;
        }

        private static void CheckSettings()
        {
            var errors = string.Empty;
            if (String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Braintree.mode"]))
                errors += "Braintree.mode is empty";
            if (String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Braintree.merchantId"]))
                errors += "Braintree.mode is empty";
            if (String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Braintree.publicKey"]))
                errors += "Braintree.mode is empty";
            if (String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Braintree.privateKey"]))
                errors += "Braintree.mode is empty";

            if (!string.IsNullOrWhiteSpace(errors))
                throw new Exception("Could not establish Braintree connection. " + errors);
        }
    }
}
