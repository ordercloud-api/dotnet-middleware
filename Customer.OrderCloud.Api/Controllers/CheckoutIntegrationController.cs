using Catalyst.Common;
using Catalyst.Common.Commands;
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
	public class CheckoutIntegrationController
	{
		private readonly ICheckoutIntegrationCommand _checkoutIntegrationCommand;

		public CheckoutIntegrationController(ICheckoutIntegrationCommand checkoutIntegrationCommand)
		{
			_checkoutIntegrationCommand = checkoutIntegrationCommand; // Inject a command class to hold my custom logic
		}

		[HttpPost, Route("shippingrates")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<ShipEstimateResponse> GetShippingRates([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
		{
			return await _checkoutIntegrationCommand.GetShippingRates(payload);
		}

		[HttpPost, Route("ordercalculate")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
		{
			return await _checkoutIntegrationCommand.CalculateOrder(payload);
		}

		[HttpPost, Route("ordersubmit")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
		{
			return await _checkoutIntegrationCommand.HandleOrderReleased(payload);
		}

		[HttpPost, Route("orderapproved")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderSubmitResponse> HandleOrderApproved([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
		{
			return await _checkoutIntegrationCommand.HandleOrderReleased(payload);
		}
	}
}
