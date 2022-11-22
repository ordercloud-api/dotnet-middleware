using OrderCloud.Catalyst;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Customer.OrderCloud.Common.Models;
using Customer.OrderCloud.Common.Commands;

namespace Customer.OrderCloud.Api.Controllers
{
	//    **************************************************************************************
	//    All routes are hit from special OrderCloud webhooks called "Message Senders".
	//    Create a Message sender config object in OrderCloud. https://ordercloud.io/api-reference/seller/message-senders/create
	//    Set MessageSender.SharedKey to match settings.OrderCloudSettings.WebhookHashKey in this project
	//    Set MessageSender.Url to "$baseUrl$/api/messagesender/{messagetype}" (include bracket literal characters) to match routes in this file
	//    Set MessageSender.MessageTypes to the types you want to use (routes below)
	//    Set MessageSender.xp to any json data you want included in the payload.ConfigData property for all routes 	
	//    See guide on message senders https://ordercloud.io/knowledge-base/message-senders
	//    **************************************************************************************
	[Route("api/messagesender")]
	public class MessageSenderController : CatalystController
	{
		private readonly ISendEmailMapperCommand _command;

		public MessageSenderController(ISendEmailMapperCommand command)
		{
			_command = command;
		}

		// These message sender types share payloads and default template data fields. They should have unqiue templates. They can be split into separate actions as needed. 
		[HttpPost, 
			Route("NewUserInvitation"), 
			Route("ForgottenPassword")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task NewUserInvitation(SetPasswordMessageSenderPayloadWithXp payload)
			=> await _command.SendSetPasswordEmail(payload);


		// These message sender types share payloads and default template data fields. They should have unqiue templates. They can be split into separate actions as needed. 
		[HttpPost, 
			Route("OrderSubmitted"),
			Route("OrderSubmittedForApproval"),
			Route("OrderApproved"),
			Route("OrderDeclined"),
			Route("OrderSubmittedForYourApproval"),
			Route("OrderSubmittedForYourApprovalHasBeenApproved"),
			Route("OrderSubmittedForYourApprovalHasBeenDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderSubmitted(OrderMessageSenderPayloadWithXp payload)
			=> await _command.SendOrderEmail(payload);


		[HttpPost, Route("ShipmentCreated")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task ShipmentCreated(ShipmentCreatedMessageSenderPayloadWithXp payload)
			=> await _command.SendShipmentCreatedEmail(payload);


		// These message sender types share payloads and default template data fields. They should have unqiue templates. They can be split into separate actions as needed. 
		[HttpPost, 
			Route("OrderReturnSubmitted"),
			Route("OrderReturnSubmittedForApproval"),
			Route("OrderReturnApproved"),
			Route("OrderReturnDeclined"),
			Route("OrderReturnSubmittedForYourApproval"),
			Route("OrderReturnSubmittedForYourApprovalHasBeenDeclined"),
			Route("OrderReturnSubmittedForYourApprovalHasBeenDeclined"),
			Route("OrderReturnCompleted")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnSubmitted(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.SendOrderReturnEmail(payload);
	}
}
