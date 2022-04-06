using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalyst.Api.Controllers
{
	// This controller demonstrates how to create routes that listen to events from the ordercloud platform, called webhooks.
	// You can configure Ordercloud to send webhook notifications to your middleware either before (a pre-webook) or after (a post-webhook) any write API request to Ordercloud.  
	// Learn about that configuration at https://ordercloud.io/knowledge-base/using-webhooks#register-your-hook-with-ordercloud.
	// When developing webhook listeners, you may want to test them before publishing to a hosted env. https://ngrok.com/ is free developer tool that supports this workflow by creating public tunnels to your local routes.
	public class WebhookController : CatalystController
	{
		private readonly IOrderCloudClient _oc;

		public WebhookController(IOrderCloudClient oc)
		{
			_oc = oc;
		}


		// This example sends a text to purchasers after their orders have been approved.
		// In ordercloud, configure a post-webhook on the order approve action.
		[HttpPost("api/webhook/orderapproved")] // Supply this route as the listener.  
		[OrderCloudWebhookAuth] // This security feature blocks requests that are not from Ordercloud. Make sure settings.OrderCloudSettings.WebhookHashKey matches whats configured in O.C.  
		// A post webhook lister needs no return type.
		public async Task HandleOrderApprove([FromBody] WebhookPayloads.Orders.Approve payload)
		{
			// In the function body, take whatever action should follow an order approve event
			var order = await _oc.Orders.GetAsync(OrderDirection.Incoming, payload.RouteParams.OrderID); // get the full order
			var user = await _oc.Users.GetAsync(order.FromCompanyID, order.FromUserID); // get the purchaser 
			var phoneNumber = user.Phone;
			var message = $"Hi {user.FirstName}, your PetPlanet order {order.ID} was just approved!";
			// Plug in a Twillio or another text service here.
		}


		// This example validates all new addresses that are created.
		// In ordercloud, configure a pre-webhook on the address create action, and supply this route as the listener.  
		[HttpPost("api/webhook/createaddress")]
		[OrderCloudWebhookAuth]
		// A pre webhook lister should return a type of PreWebhookResponse 
		public PreWebhookResponse HandleAddressCreate([FromBody] WebhookPayloads.Addresses.Create payload) 
		{
			var address = payload.Request.Body;
			var isAddressValid = false; // Plug in an address validation service here. 
			List<Address> suggestedAddresses = null;
			if (isAddressValid)
			{
				// The address create action will go forward in Ordercloud.
				return PreWebhookResponse.Proceed(); 
			}
			else
			{
				// The action will be blocked, and the suggested Addresses returned in the body of the original request.
				return PreWebhookResponse.Block(new { Suggested = suggestedAddresses }); 
			}
		}
	}
}
