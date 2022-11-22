using OrderCloud.Catalyst;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Customer.OrderCloud.Common.Models;
using Customer.OrderCloud.Common.Commands;

namespace Customer.OrderCloud.Api.Controllers
{
	//    **************************************************************************************
	//    All routes are hit from special OrderCloud webhooks called "Integration Events".
	//    Create an IntegrationEvent config object in OrderCloud for each event type (AddToCart, OrderCheckout, OrderReturn, OpenIdConnect) you wish to use.
	//    See https://ordercloud.io/api-reference/seller/integration-events/create
	//    For all config objects, set IntegrationEvent.HashKey to match match settings.OrderCloudSettings.WebhookHashKey.
	//    **************************************************************************************

	[Route("api/integrationevents")]
	public class IntegrationEventController : CatalystController
	{
		private readonly IAddToCartEventCommand _addToCartCommand;
		private readonly IOpenIdConnectCommand _openIdConnectCommand;
		private readonly IOrderReturnCommand _orderReturnCommand;
		private readonly ICheckoutCommand _checkoutCommand;

		public IntegrationEventController(
			IAddToCartEventCommand addToCartCommand, 
			IOpenIdConnectCommand openIdConnectCommand, 
			IOrderReturnCommand orderReturnCommand,
			ICheckoutCommand checkoutCommand
			)
		{
			_addToCartCommand = addToCartCommand;
			_openIdConnectCommand = openIdConnectCommand;
			_orderReturnCommand = orderReturnCommand;
			_checkoutCommand = checkoutCommand;
		}

		//    **************************************************************************************
		//    *  EventType - "AddToCard"
		//    *  CustomImplementationUrl - "{baseUrl}/api/integrationevents/addtocart"
		//    *  For integrating an external product catalog with ordercloud cart and fulfillment.
		//    *  See https://ordercloud.io/knowledge-base/ad-hoc-products
		//    **************************************************************************************

		[HttpPost, Route("addtocart")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<AddToCartResponseWithXp> GetProductWithUnitPriceAsync([FromBody] AddToCartIEPayloadWithXp payload) =>
			await _addToCartCommand.GetProductWithUnitPriceAsync(payload);

		//    **************************************************************************************
		//    *  EventType - "OpenIDConnect"
		//    *  CustomImplementationUrl - "{baseUrl}/api/integrationevents/openidconnect"
		//    *  For single sign on integrations where users need to be synced into OrderCloud from an identity provider
		//    *  See https://ordercloud.io/knowledge-base/sso-via-openid-connect
		//    **************************************************************************************

		[HttpPost, Route("openidconnect/createuser")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OpenIdConnectCreateUserResponse> CreateUserFromSSOAsync([FromBody] OpenIDConnectIEPayloadWithXp payload) =>
			await _openIdConnectCommand.CreateUserFromSSOAsync(payload);

		[HttpPost, Route("openidconnect/syncuser")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OpenIdConnectSyncUserResponse> UpdateUserFromSSOAsync([FromBody] OpenIDConnectIEPayloadWithXp payload) =>
			await _openIdConnectCommand.UpdateUserFromSSOAsync(payload);

		//    **************************************************************************************
		//    *  EventType - "OrderReturn"
		//    *  CustomImplementationUrl - "{baseUrl}/api/integrationevents/orderreturn"
		//    *  For custom calculation of refund amounts on order returns
		//    *  See https://ordercloud.io/knowledge-base/order-returns
		//    **************************************************************************************

		[HttpPost, Route("orderreturn/calculateorderreturn")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderReturnResponse> CalculateOrderReturnRefundAsync([FromBody] OrderReturnIEPayloadWithXp payload) =>
			await _orderReturnCommand.CalculateOrderReturnRefundAsync(payload);

		//    **************************************************************************************
		//    *  EventType - "OrderCheckout"
		//    *  CustomImplementationUrl - "{baseUrl}/api/integrationevents/ordercheckout"
		//    *  For all checkout integrations like shipping, tax, payment, email confirmation, order forwarding 
		//    *  See https://ordercloud.io/knowledge-base/order-checkout-integration
		//    **************************************************************************************

		[HttpPost, Route("ordercheckout/shippingrates")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<ShipEstimateResponseWithXp> EstimateShippingCostsAsync([FromBody] OrderCheckoutIEPayloadWithXp payload) =>
			await _checkoutCommand.EstimateShippingCostsAsync(payload);

		[HttpPost, Route("ordercheckout/ordercalculate")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderCalculateResponseWithXp> RecalculatePricesAndTaxAsync([FromBody] OrderCheckoutIEPayloadWithXp payload) =>
			await _checkoutCommand.RecalculatePricesAndTaxAsync(payload);

		[HttpPost, Route("ordercheckout/ordersubmit")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderSubmitResponseWithXp> PostSubmitProcessingAsync([FromBody] OrderCheckoutIEPayloadWithXp payload) =>
			await _checkoutCommand.ProcessOrderPostSubmitAsync(payload);

		[HttpPost, Route("ordercheckout/orderapproved")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderApprovedResponseWithXp> PostApprovalProcessingAsync([FromBody] OrderCheckoutIEPayloadWithXp payload) =>
			await _checkoutCommand.ProcessOrderPostApprovalAsync(payload);

		[HttpPost, Route("ordercheckout/ordersubmitforapproval")] // route and method specified by OrderCloud platform
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task<OrderSubmitForApprovalResponseWithXp> PostSubmitForApprovalProcessingAsync([FromBody] OrderCheckoutIEPayloadWithXp payload) =>
			await _checkoutCommand.ProcessOrderPostSubmitForApprovalAsync(payload);

	}
}
