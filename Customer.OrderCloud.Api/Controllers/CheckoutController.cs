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

namespace Customer.OrderCloud.Api.Controllers
{
	// See an overview of integration events at https://ordercloud.io/knowledge-base/how-to-calculate-tax-with-ordercloud#integration-event
	// To wire up these listeners in OrderCloud create an IntegrationEvent object of type "OrderCheckout".
	// https://ordercloud.io/api-reference/seller/integration-events/create
	// Set CustomImplementationUrl to the url of this API project. 
	// Set HashKey to match match settings.OrderCloudSettings.WebhookHashKey.
	// Set ConfigData to any object you want passed into all requests as payload.ConfigData. 
	// Set ElevatedRoles to roles that payload.OrderCloudAccessToken should have. 
	[OrderCloudIntegrationEvent(IntegrationEventType.OrderCheckout)] // This tags the controller as containing an integration event for auto-generated configs
	public class CheckoutController: CatalystController
	{
		private readonly ICheckoutCommand _checkoutCommand;
		private readonly IOrderCloudClient _oc;

		public CheckoutController(ICheckoutCommand checkoutCommand, IOrderCloudClient oc)
		{
			_checkoutCommand = checkoutCommand; // Inject a command class to hold custom logic
			_oc = oc;
		}

		// Hit from OC Integration Event Webhook
		[HttpPost, Route("shippingrates")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<ShipEstimateResponseWithXp> EstimateShippingCostsAsync([FromBody] OrderCalculatePayloadWithXp payload) =>
			await _checkoutCommand.EstimateShippingCostsAsync(payload);


		// Hit from OC Integration Event Webhook
		[HttpPost, Route("ordercalculate")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderCalculateResponseWithXp> RecalculatePricesAndTaxAsync([FromBody] OrderCalculatePayloadWithXp payload) =>
			await _checkoutCommand.RecalculatePricesAndTaxAsync(payload);

		// Hit from Storefront Client
		[HttpPut, Route("me/card-payment")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<PaymentWithXp> SetCreditCardPaymentAsync(CreditCardPayment payment)
		{
			var shopper = await _oc.Me.GetAsync<MeUserWithXp>(UserContext.AccessToken);
			return await _checkoutCommand.SetCreditCardPaymentAsync(shopper, payment);
		}
			
		// Hit from Storefront Client
		[HttpPost, Route("order/{orderID}/submit")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<OrderConfirmation> SubmitOrderAsync(string orderID) =>
			await _checkoutCommand.SubmitOrderAsync(orderID, UserContext);

		// Hit from OC Integration Event Webhook
		[HttpPost, Route("ordersubmit")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderSubmitResponseWithXp> PostSubmitProcessingAsync([FromBody] OrderCalculatePayloadWithXp payload) =>
			await _checkoutCommand.ProcessOrderPostSubmitAsync(payload);
	}
}
