using Catalyst.Common;
using Catalyst.Common.Services;
using Customer.OrderCloud.Common.Models;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
    // Data configured in OrderCloud to be passed to all Checkout Integration Events as Payload.ConfigData
    public class CheckoutConfig
    {
        public string MyProperty { get; set; }
    }

    public interface ICheckoutCommand
    {
        Task<ShipEstimateResponse> EstimateShippingAsync(OrderCalculatePayload<CheckoutConfig> payload);
		Task<OrderCalculateResponse> RecalculateOrderAsync(OrderCalculatePayload<CheckoutConfig> payload);
        Task<List<SavedCreditCard>> ListSavedCreditCardsAsync();
		Task<PaymentWithXp> CreateCreditCardPaymentAsync(CreditCardPayment payment);
        Task<OrderConfirmation> SubmitOrderAsync(string orderID);
        Task<OrderSubmitResponse> ProcessSubmittedOrderAsync(OrderCalculatePayload<CheckoutConfig> payload);
    }

    public class CheckoutCommand : ICheckoutCommand
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
		private readonly IAzureServiceBus _serviceBus;
        private readonly IShipMethodCalculator _shippingCalculator;
		private readonly ITaxCalculator _taxCalculator;
        private readonly ICreditCardCommand _creditCardCommand;
        private readonly RequestAuthenticationService _authentication;

        public CheckoutCommand(
            IOrderCloudClient oc, 
            AppSettings settings, 
            IAzureServiceBus serviceBus, 
            ITaxCalculator taxCalculator, 
            IShipMethodCalculator shippingCalculator,
            ICreditCardCommand creditCardCommand,
            RequestAuthenticationService authentication
            )
        {
            _serviceBus = serviceBus;
            _oc = oc;
            _settings = settings;
            _taxCalculator = taxCalculator;
            _shippingCalculator = shippingCalculator;
            _creditCardCommand = creditCardCommand;
            _authentication = authentication;
        }

        public async Task<ShipEstimateResponse> EstimateShippingAsync(OrderCalculatePayload<CheckoutConfig> payload)
        {
            return null;
        }

        public async Task<OrderCalculateResponse> RecalculateOrderAsync(OrderCalculatePayload<CheckoutConfig> payload)
        {
            return null;
        }

        public async Task<List<SavedCreditCard>> ListSavedCreditCardsAsync()
		{
            var shopper = await _authentication.GetUserAsync<MeUserWithXp>();
            var cards = await _creditCardCommand.ListSavedCardsAsync(shopper);
            return cards;
        }

        public async Task<PaymentWithXp> CreateCreditCardPaymentAsync(CreditCardPayment ccPayment)
        {
            var shopper = await _authentication.GetUserAsync<MeUserWithXp>();
            PCISafeCardDetails safeCardDetails;
            if (ccPayment.CardDetails != null)
			{
                if (ccPayment.SaveCardDetailsForFutureUse)
				{
                    // using a new CC and saving it
                    safeCardDetails = await _creditCardCommand.CreateSavedCardAsync(shopper, ccPayment.CardDetails);
                } else
				{
                    // one-time use of CC
                    safeCardDetails = ccPayment.CardDetails; 
                }
			} else if (ccPayment.SavedCardID != null)
			{
                // using a saved CC
                safeCardDetails = await _creditCardCommand.GetSavedCardAsync(shopper, ccPayment.SavedCardID);
            } else
			{
                throw new CatalystBaseException("PaymentDetailsMissing", "Create credit card payment must have either non-null CardDetails or SavedCardID", null, HttpStatusCode.BadRequest);
			}
			var payment = new PaymentWithXp()
			{
				Type = PaymentType.CreditCard,
				Amount = ccPayment.Amount,
				Accepted = false,
				xp = new PaymentXp()
				{
					SafeCardDetails = safeCardDetails
                }
			};
            var createdPayment = await _oc.Payments.CreateAsync<PaymentWithXp>(ccPayment.OrderDirection, ccPayment.OrderID, payment);
            return createdPayment;
            // TODO - think about if payment already exists
        }

		public async Task<OrderConfirmation> SubmitOrderAsync(string orderID)
		{
            return null;
        }

        public async Task<OrderSubmitResponse> ProcessSubmittedOrderAsync(OrderCalculatePayload<CheckoutConfig> payload)
		{
            return null;
        }
    }
}
