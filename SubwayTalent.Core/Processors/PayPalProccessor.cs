using PayPal.Api;
using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SubwayTalent.Core.Processors
{
    public class PayPalProcessor : IPayPalProcessor, IPaymentProcessor
    {

        private string accessToken = string.Empty;
        private IDictionary<string, string> _settings;
        private float _handlingFee;
        private float _adminFee;
        private float _adminTaxFee;
        private float _paypalFee;

        public PayPalProcessor() { }
        public PayPalProcessor(IDictionary<string, string> settings)
        {
            _settings = settings;
        }

        public string CreateRefreshToken(string authorizationCode)
        {
            var apiContext = PayPalConfig.GetAPIContext();

            var authorizationCodeParameters = new CreateFromAuthorizationCodeParameters();
            authorizationCodeParameters.setClientId(PayPalConfig.ClientId);
            authorizationCodeParameters.setClientSecret(PayPalConfig.ClientSecret);
            authorizationCodeParameters.SetCode(authorizationCode);

            var tokenInfo = Tokeninfo.CreateFromAuthorizationCodeForFuturePayments(apiContext, authorizationCodeParameters);

            return tokenInfo.refresh_token;
        }

        public string CreateAccessToken(string refreshToken)
        {
            var apiContext = PayPalConfig.GetAPIContext();

            var tokenInfo = new Tokeninfo();
            tokenInfo.refresh_token = refreshToken;

            var dict = new Dictionary<string, string>();

            dict.Add("grant_type", "refresh_token");

            var accessToken = tokenInfo.CreateFromRefreshToken(apiContext, new CreateFromRefreshTokenParameters
            {
                clientId = PayPalConfig.ClientId,
                clientSecret = PayPalConfig.ClientSecret,
                ContainerMap = dict

            });

            return accessToken.access_token;
        }

        public string CreatePayment(float total, string token)
        {
            try
            {

                var totalPlannerPayments = ComputeTotalFee(total);

                //tax
                var adminTaxPercentage = _settings.ContainsKey("admin_tax_percentage") ? float.Parse(_settings["admin_tax_percentage"]) / 100 : 0;
                _adminTaxFee = (total + _adminFee) * adminTaxPercentage;

                //crate access token from refresh token
                accessToken = CreateAccessToken(token);

                var createPaymentApiContext = PayPalConfig.GetAPIContext("Bearer " + accessToken);

                Details details = PopolateDetails(totalPlannerPayments.ToString("0.00"), _adminTaxFee.ToString("0.00"), _handlingFee.ToString("0.00"));

                var futurePayment = new FuturePayment
                {
                    intent = "authorize",
                    payer = new Payer()
                    {
                        payment_method = "paypal"
                    },
                    transactions = new List<Transaction>()
                {
                    new Transaction()
                    {                        
                        amount = new Amount()
                        {
                            currency = "USD",                           
                            total = totalPlannerPayments.ToString("0.00"),
                            details = details
                        },
                        description = "Payment for Subway Talent Inc.",                      
                        note_to_payee="The paypal fee was shouldered by the planner which was added in the handling fee. " +
                                        "The % set in the database is 2.9% + 0.3USD."                
                    }
                }
                };

                var fPayment = futurePayment.Create(createPaymentApiContext);

                return fPayment.transactions[0].related_resources[0].authorization.id;
            }
            catch (PayPal.HttpException pex)
            {
                throw new Exception(((PayPal.HttpException)pex).Response);
            }
        }

        public string CapturePayment(float total, string authorizationId, string token)
        {

            //crate access token from refresh token
            accessToken = CreateAccessToken(token);

            var capturePaymentApiContext = PayPalConfig.GetAPIContext("Bearer " + accessToken);
            var totalPlannerPayments = ComputeTotalFee(total);

            Authorization auth = new Authorization();
            auth.id = authorizationId;

            var capture = new Capture()
            {
                amount = new Amount
                {
                    currency = "USD",
                    total = totalPlannerPayments.ToString("0.00")
                },

                is_final_capture = true
            };

            var responseCapture = auth.Capture(capturePaymentApiContext, capture);


            if (responseCapture.state.ToLower() != "completed")
                throw new Exception("Failed to capture payment. ReasonCode[" + responseCapture.reason_code + "]");

            return responseCapture.id;
        }

        private Details PopolateDetails(string total, string tax, string handlingFee)
        {
            Details details = new Details();

            float taxValue = 0;
            float subTotalValue = 0;
            float totalValue = 0;
            float handlingFeeValue = 0;

            if (!float.TryParse(total, out totalValue))
                throw new Exception("invalid total value for transaction");

            details.tax = !string.IsNullOrWhiteSpace(tax) ? (!float.TryParse(tax, out taxValue) ? "0" : taxValue.ToString()) : "0";
            details.handling_fee = !string.IsNullOrWhiteSpace(handlingFee) ? (!float.TryParse(handlingFee, out handlingFeeValue) ? "0" : handlingFeeValue.ToString()) : "0";

            subTotalValue = totalValue - (taxValue + handlingFeeValue);

            details.subtotal = subTotalValue.ToString();
            details.shipping = "0";

            return details;
        }

        private float ComputeTotalFee(float total)
        {
            //admin fee
            var adminPayPercent = _settings.ContainsKey("admin_pay_percentage") ? float.Parse(_settings["admin_pay_percentage"]) / 100 : 0;
            _adminFee = adminPayPercent * total;

            //paypal fee
            var paypalPercentage = _settings.ContainsKey("paypal_percentage") ? float.Parse(_settings["paypal_percentage"]) / 100 : 0;
            var paypalAdditional = _settings.ContainsKey("paypal_additional") ? float.Parse(_settings["paypal_additional"]) : 0;
            _paypalFee = (paypalPercentage * (total + _adminFee)) + paypalAdditional;

            //the planner will shoulder the admin fee and paypalFee.
            _handlingFee = _adminFee + _paypalFee;


            return (total + _handlingFee);
        }


        public IDictionary<string,string> CreatePaymetMethod(string authorizationCode, string firstName, string lastName, string email)
        {
            var paymentInfo = new Dictionary<string, string>();
            paymentInfo.Add("paymentToken", this.CreateRefreshToken(authorizationCode));

            return paymentInfo;
        }
    }
}