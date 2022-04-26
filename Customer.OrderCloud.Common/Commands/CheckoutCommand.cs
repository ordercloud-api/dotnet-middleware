﻿using Catalyst.Common;
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
        Task<ShipEstimateResponseWithXp> EstimateShippingCostsAsync(OrderCalculatePayloadWithXp payload);
		Task<OrderCalculateResponseWithXp> RecalculatePricesAndTaxAsync(OrderCalculatePayloadWithXp payload);
        Task<List<PCISafeCardDetails>> ListSavedCreditCardsAsync();
	    Task<PaymentWithXp> CreateCreditCardPaymentAsync(CreditCardPayment payment);
        Task<OrderConfirmation> SubmitOrderAsync(string orderID, DecodedToken shopperToken);
        Task<OrderSubmitResponseWithXp> ProcessOrderPostSubmitAsync(OrderCalculatePayloadWithXp payload);
    }

    public class CheckoutCommand : ICheckoutCommand
    {
        private readonly AppSettings _settings;
        private readonly IOrderCloudClient _oc;
	    private readonly IAzureServiceBus _serviceBus;
        private readonly IShippingRatesCalculator _shippingCalculator;
	    private readonly ITaxCalculator _taxCalculator;
        private readonly ICreditCardCommand _creditCardCommand;
        private readonly RequestAuthenticationService _authentication;

        public CheckoutCommand(
            IOrderCloudClient oc, 
            AppSettings settings, 
            IAzureServiceBus serviceBus, 
            ITaxCalculator taxCalculator,
            IShippingRatesCalculator shippingCalculator,
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

        public async Task<ShipEstimateResponseWithXp> EstimateShippingCostsAsync(OrderCalculatePayloadWithXp payload)
        {
            var shipments = ContainerizeLineItems(payload);
            var packages = shipments.Select(shipment => shipment.ShipPackage).ToList();
            var shipMethodOptions = await _shippingCalculator.CalculateShippingRatesAsync(packages);
	        var response = new ShipEstimateResponseWithXp()
	        {
	    	    ShipEstimates = shipments.Select((shipment, index) =>
		        {
                    return new ShipEstimateWithXp()
                    {
                        ShipMethods = shipMethodOptions[index].Select(sm => new ShipMethodWithXp() 
                        {
                           ID = sm.ID,
                           Name = sm.Name,
                           Cost = sm.Cost,
                           EstimatedTransitDays = sm.EstimatedTransitDays,
                           xp = new ShipMethodXp() {
                               Carrier = sm.Carrier
                           }
                        }).ToList(),
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

	    private List<ShippingPackageWithLineItems> ContainerizeLineItems(OrderCalculatePayloadWithXp payload)
	    {
            return null;
	    }

        public async Task<OrderCalculateResponseWithXp> RecalculatePricesAndTaxAsync(OrderCalculatePayloadWithXp payload)
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
                ShippingCosts = payload.OrderWorksheet.ShipEstimateResponse.ShipEstimates.Select(se =>
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

        public async Task<List<PCISafeCardDetails>> ListSavedCreditCardsAsync()
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
            var createdPayment = await _oc.Payments.CreateAsync<PaymentWithXp>(OrderDirection.All, ccPayment.OrderID, payment);
            return createdPayment;
			// TODO - think about if payment already exists
		}

		public async Task<OrderConfirmation> SubmitOrderAsync(string orderID, DecodedToken shopperToken)
		{
			var worksheet = await _oc.IntegrationEvents.GetWorksheetAsync<OrderWorksheetWithXp>(OrderDirection.All, orderID);
			var payments = (await _oc.Payments.ListAsync<PaymentWithXp>(OrderDirection.All, orderID)).Items.ToList();
			await ValidateOrder(worksheet, payments, shopperToken);
            var ccPayment = payments.First(IsCreditCardPayment); // validate should have thrown an error if this doesn't exist

            ccPayment = await _creditCardCommand.AuthorizeCardPayment(worksheet, ccPayment);

			try
			{
				await _oc.Orders.SubmitAsync<OrderWithXp>(OrderDirection.All, orderID, shopperToken.AccessToken);
                return new OrderConfirmation()
                {
                    OrderWorksheet = worksheet,
                    Payments = payments
                };
			}
			catch (Exception)
            {
                await _creditCardCommand.VoidCardPayment(worksheet.Order.ID, ccPayment);
                throw;
            }
        }

        private async Task ValidateOrder(OrderWorksheetWithXp worksheet, List<PaymentWithXp> payments, DecodedToken shopperToken)
		{
            Require.That(
               !worksheet.Order.IsSubmitted,
               new ErrorCode("OrderSubmit.AlreadySubmitted", "Order has already been submitted")
            );
            Require.That(
                worksheet.OrderCalculateResponse != null &&
                worksheet.OrderCalculateResponse.HttpStatusCode == 200 &&
                worksheet?.OrderCalculateResponse?.xp != null &&
                worksheet?.OrderCalculateResponse?.xp.TaxDetails != null,
                new ErrorCode("OrderSubmit.OrderCalculateError", "A problem occurred during Order Calculation.  Please go back to the cart and try to checkout again.")
            );

            var shipMethodsWithoutSelections = worksheet?.ShipEstimateResponse?.ShipEstimates?.Where(estimate => estimate.SelectedShipMethodID == null);
            Require.That(
                worksheet?.ShipEstimateResponse != null &&
                shipMethodsWithoutSelections.Count() == 0,
                new ErrorCode("OrderSubmit.MissingShippingSelections", "All shipments on an order must have a selection"), shipMethodsWithoutSelections
			);

			Require.That(
			    payments.Exists(IsCreditCardPayment),
			    new ErrorCode("OrderSubmit.MissingPayment", "Order must include credit card payment details")
			);
            var inactiveLineItems = await FindInactiveLineItems(worksheet, shopperToken.AccessToken);
            Require.That(
                !inactiveLineItems.Any(),
                new ErrorCode("OrderSubmit.InvalidProducts", "Order contains line items for products that are inactive"), inactiveLineItems
            );
            try
            {
                // ordercloud validates the same stuff that would be checked on order submit
                await _oc.Orders.ValidateAsync(OrderDirection.Incoming, worksheet.Order.ID);

                // TODO - check promotions and pricing are still valid per OC-Platform conversation with Miranda.
            }
            catch (OrderCloudException ex)
            {
                // this error is expected and will be resolved before oc order submit call happens
                var errors = ex.Errors.Where(ex => ex.ErrorCode != "Order.CannotSubmitWithUnacceptedPayments");
                if (errors.Any())
                {
                    throw new CatalystBaseException(new ApiError
                    {
                        ErrorCode = "OrderSubmit.OrderCloudValidationError",
                        Message = "Failed ordercloud validation, see Data for details",
                        Data = errors
                    });
                }
            }
        }

        private bool IsCreditCardPayment(PaymentWithXp payment) => payment.Type == PaymentType.CreditCard && payment.xp.SafeCardDetails.Token != null;

        private async Task<List<LineItemWithXp>> FindInactiveLineItems(OrderWorksheetWithXp worksheet, string userToken)
		{
			List<LineItemWithXp> inactiveLineItems = new List<LineItemWithXp>();
            await Throttler.RunAsync(worksheet.LineItems, 100, 8, async lineItem =>
            {
                try
                {
                    await _oc.Me.GetProductAsync(lineItem.ProductID, accessToken: userToken);
                }
                catch (OrderCloudException ex) when (ex.HttpStatus == HttpStatusCode.NotFound)
                {
                    inactiveLineItems.Add(lineItem);
                }
            });
            return inactiveLineItems;
        }

        public async Task<OrderSubmitResponseWithXp> ProcessOrderPostSubmitAsync(OrderCalculatePayloadWithXp payload)
		{
            return null;
        }
    }
}