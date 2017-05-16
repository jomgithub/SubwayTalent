using Braintree;
using SubwayTalent.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubwayTalent.Core.Processors
{
    public class BraintreeProcessor : IBraintreeProcessor, IPaymentProcessor
    {
        private IDictionary<string, string> _settings;
        private float _handlingFee;
        private float _adminFee;
        private float _adminTaxFee;
        private float _braintreeFee;

        public BraintreeProcessor() { }
       
        public BraintreeProcessor(IDictionary<string, string> settings)
        {
            _settings = settings;
        }
        public Dictionary<string, string> CreateCustomer(string paymentNonce, string firstName, string lastName, string email)
        {
            var request = new CustomerRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                CreditCard = new CreditCardRequest
                {
                    PaymentMethodNonce = paymentNonce,
                    Options = new CreditCardOptionsRequest
                    {
                        VerifyCard = true
                    }
                }
            };

            Result<Customer> result = BraintreeConfig.Gateway().Customer.Create(request);
            bool success = result.IsSuccess();
            if (success)
            {
                var cardInfo = new Dictionary<string, string>();
                cardInfo.Add("customerId", result.Target.CreditCards[0].CustomerId);
                cardInfo.Add("paymentToken", result.Target.CreditCards[0].Token);
                cardInfo.Add("maskedCard", result.Target.CreditCards[0].MaskedNumber);
                cardInfo.Add("cardType", result.Target.CreditCards[0].CardType.ToString());
                return cardInfo;
            }

            var errors = string.Empty;

            foreach (var error in result.Errors.All())
            {
                errors += string.Format("Code[{0}] Message[{1}]. ", error.Code.ToString(), error.Message);
            }
            foreach (var error in result.Errors.DeepAll())
            {
                errors += string.Format("Code[{0}] Message[{1}]. ", error.Code.ToString(), error.Message);
            }

            throw new Exception(errors);
        }

        public string CreatePayment(float total, string token)
        {

            Result<PaymentMethodNonce> result = BraintreeConfig.Gateway().PaymentMethodNonce.Create(token);
            bool success = result.IsSuccess();

            if (!success)
                throw new Exception("Failed to create payment nonce for accesstoken[" + token + "]");

            var totalPlannerPayments = ComputeTotalFee(total);

            TransactionRequest transactionRequest = new TransactionRequest
                {
                    Amount = decimal.Parse(totalPlannerPayments.ToString("0.00")),
                    PaymentMethodNonce = result.Target.Nonce
                };

            Result<Transaction> result2 = BraintreeConfig.Gateway().Transaction.Sale(transactionRequest);

            success = result2.IsSuccess();

            if (!success)
            {
                var errors = string.Empty;

                foreach (var error in result2.Errors.All())
                {
                    errors += string.Format("Code[{0}] Message[{1}]. ", error.Code.ToString(), error.Message);
                }
                foreach (var error in result2.Errors.DeepAll())
                {
                    errors += string.Format("Code[{0}] Message[{1}]. ", error.Code.ToString(), error.Message);
                }
                throw new Exception("Failed to create payment for accesstoken [" + token + "]" + errors);
            }

            return result2.Target.Id;
        }

        public string CapturePayment(float total, string authorizationId, string token)
        {
            Result<Transaction> result = BraintreeConfig.Gateway().Transaction.SubmitForSettlement(authorizationId);
            bool success = result.IsSuccess();

            if (!success)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors.All())
                {
                    errors += string.Format("Code[{0}] Message[{1}]. ", error.Code.ToString(), error.Message);
                }
                foreach (var error in result.Errors.DeepAll())
                {
                    errors += string.Format("Code[{0}] Message[{1}]. ", error.Code.ToString(), error.Message);
                }

                throw new Exception("Failed to create payment for transaction [" + authorizationId + "]" + errors);
            }

            return result.Target.Id;
        }

        private float ComputeTotalFee(float total)
        {
            //admin fee
            var adminPayPercent = _settings.ContainsKey("admin_pay_percentage") ? float.Parse(_settings["admin_pay_percentage"]) / 100 : 0;
            _adminFee = adminPayPercent * total;

            //braintree fee
            var braintreePercentage = _settings.ContainsKey("braintree_percentage") ? float.Parse(_settings["braintree_percentage"]) / 100 : 0;
            var braintreeAdditional = _settings.ContainsKey("braintree_additional") ? float.Parse(_settings["braintree_additional"]) : 0;
            _braintreeFee = (braintreePercentage * (total + _adminFee)) + braintreeAdditional;

            //the planner will shoulder the admin fee and braintreeFee.
            _handlingFee = _adminFee + _braintreeFee;


            return (total + _handlingFee); 
        }

        public IDictionary<string, string> CreatePaymetMethod(string authorizationCode, string firstName, string lastName, string email)
        {
            return this.CreateCustomer(authorizationCode, firstName, lastName, email);
        }
    }
}