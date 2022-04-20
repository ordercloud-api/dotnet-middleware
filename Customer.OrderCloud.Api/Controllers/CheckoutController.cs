using Catalyst.Common;
using Customer.OrderCloud.Common.Commands;
using Customer.OrderCloud.Common.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalyst.Api.Controllers
{
	// See an overview of integration events at https://ordercloud.io/knowledge-base/how-to-calculate-tax-with-ordercloud#integration-event
	// To wire up these listeners in OrderCloud create an IntegrationEvent object of type "OrderCheckout".
	// https://ordercloud.io/api-reference/seller/integration-events/create
	// Set CustomImplementationUrl to the url of this API project. 
	// Set HashKey to match match settings.OrderCloudSettings.WebhookHashKey.
	// Set ConfigData to any object you want passed into all requests as payload.ConfigData. 
	// Set ElevatedRoles to roles that payload.OrderCloudAccessToken should have. 
	public class CheckoutController
	{
		private readonly ICheckoutCommand _checkoutCommand;

		public CheckoutController(ICheckoutCommand checkoutCommand)
		{
			_checkoutCommand = checkoutCommand; // Inject a command class to hold my custom logic
		}

		[HttpPost, Route("shippingrates")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<ShipEstimateResponseWithXp> EstimateShippingCostsAsync([FromBody] OrderCalculatePayloadWithXp payload)
		{
			return await _checkoutCommand.EstimateShippingCostsAsync(payload);
		}

		[HttpPost, Route("ordercalculate")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderCalculateResponseWithXp> RecalculatePricesAndTaxAsync([FromBody] OrderCalculatePayloadWithXp payload)
		{
			return await _checkoutCommand.RecalculatePricesAndTaxAsync(payload);
		}

		[HttpPost, Route("card-payment")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<PaymentWithXp> CreateCreditCardPaymentAsync(CreditCardPayment payment)
		{
			return await _checkoutCommand.CreateCreditCardPaymentAsync(payment);
		}

		[HttpPost, Route("order/{orderID}/submit")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<OrderConfirmation> SubmitOrderAsync(string orderID)
		{
			return await _checkoutCommand.SubmitOrderAsync(orderID);
		}

		[HttpPost, Route("ordersubmit")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderSubmitResponseWithXp> PostSubmitProcessingAsync([FromBody] OrderCalculatePayloadWithXp payload)
		{
			return await _checkoutCommand.ProcessOrderPostSubmitAsync(payload);
		}
	}
}
