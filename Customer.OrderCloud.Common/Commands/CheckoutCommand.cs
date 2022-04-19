using Catalyst.Common;
using Catalyst.Common.Services;
using Customer.OrderCloud.Common.Models;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
    public interface ICheckoutCommand
    {
        Task<ShipEstimateResponseWithXp> EstimateShippingAsync(OrderCalculatePayloadWithXp payload);
		Task<OrderCalculateResponseWithXp> RecalculateOrderAsync(OrderCalculatePayloadWithXp payload);
        Task<List<SavedCreditCard>> ListSavedCreditCardsAsync();
		Task<PaymentWithXp> CreateCreditCardPaymentAsync(CreditCardPayment payment);
        Task<OrderConfirmation> SubmitOrderAsync(string orderID);
        Task<OrderSubmitResponseWithXp> ProcessSubmittedOrderAsync(OrderCalculatePayloadWithXp payload);
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

        public async Task<ShipEstimateResponseWithXp> EstimateShippingAsync(OrderCalculatePayloadWithXp payload)
        {
            var shipments = ContainerizeLineItems(payload);
            var packages = shipments.Select(shipment => shipment.ShipPackage).ToList();
            var shipMethodOptions = await _shippingCalculator.CalculateShipMethodsAsync(packages);
			var response = new ShipEstimateResponseWithXp()
			{
				ShipEstimates = shipments.Select((shipment, index) =>
				{
                    return new ShipEstimateWithXp()
                    {
                        ShipMethods = shipMethodOptions[index].Select(sm => (ShipMethodWithXp) sm).ToList(),
                        ShipEstimateItems = shipment.ShipEstimateItems,
                        xp = new ShipEstimateXp 
                        {
                            ShipPackage = packages[index]
                        }
					};
				}).ToList()
			};
            return response;
		}

		private List<ShipPackageWithLineItems> ContainerizeLineItems(OrderCalculatePayloadWithXp payload)
		{
            return null;
		}

        public async Task<OrderCalculateResponseWithXp> RecalculateOrderAsync(OrderCalculatePayloadWithXp payload)
		{
            var summary = MapOrderToTaxSummary(payload);
            var tax = await _taxCalculator.CalculateEstimateAsync(summary);
            var response = new OrderCalculateResponseWithXp()
            {
                TaxTotal = tax.TotalTax,
                xp = new OrderCalculateResponseXp
				{
                    TaxDetails = tax
                }
            };
            return response;
        }

        private OrderSummaryForTax MapOrderToTaxSummary(OrderCalculatePayloadWithXp payload)
        {
            var taxDetails = new OrderSummaryForTax()
            {
                OrderID = payload.OrderWorksheet.Order.ID,
                CustomerCode = payload.OrderWorksheet.Order.FromUserID,
                PromotionDiscount = 0,
                LineItems = payload.OrderWorksheet.LineItems.Select(li =>
                {
                    return new LineItemSummaryForTax()
                    {
                        LineItemID = li.ID,
                        ProductID = li.ProductID,
                        ProductName = li.Product.Name,
                        Quantity = li.Quantity,
                        UnitPrice = li.UnitPrice ?? 0,
                        PromotionDiscount = li.PromotionDiscount,
                        LineTotal = li.LineTotal,
                        TaxCode = li.Product.xp.TaxCode,
                        ShipFrom = li.ShipFromAddress,
                        ShipTo = li.ShippingAddress
                    };
                }).ToList(),
                ShipEstimates = payload.OrderWorksheet.ShipEstimateResponse.ShipEstimates.Select(se =>
                {
                    var selectedMethod = se.GetSelectedShipMethod();
                    return new ShipEstimateSummaryForTax()
                    {
                        ShipEstimateID = se.ID,
                        Description = selectedMethod.Name,
                        Cost = selectedMethod.Cost,

                    };
                }).ToList()
            };
            return taxDetails;
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
                    // entering a new CC and saving it
                    safeCardDetails = await _creditCardCommand.CreateSavedCardAsync(shopper, ccPayment.CardDetails);
                } else
				{
                    // one time use of CC
                    safeCardDetails = ccPayment.CardDetails; 
                }
			} else if (ccPayment.SavedCardID != null)
			{
                // selecting a saved CC
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

        public async Task<OrderSubmitResponseWithXp> ProcessSubmittedOrderAsync(OrderCalculatePayloadWithXp payload)
		{
            return null;
        }
    }
}
