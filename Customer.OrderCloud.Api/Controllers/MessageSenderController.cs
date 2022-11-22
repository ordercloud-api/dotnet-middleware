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
		private readonly ISendEmailCommand _command;

		public MessageSenderController(ISendEmailCommand command)
		{
			_command = command;
		}

		[HttpPost, Route("NewUserInvitation")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task NewUserInvitation(SetPasswordMessageSenderPayloadWithXp payload)
			=> await _command.NewUserInvitation(payload);

		[HttpPost, Route("ForgottenPassword")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task ForgottenPassword(SetPasswordMessageSenderPayloadWithXp payload)
			=> await _command.ForgottenPassword(payload);


		[HttpPost, Route("OrderSubmitted")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderSubmitted(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderSubmitted(payload);


		[HttpPost, Route("OrderSubmitted")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderSubmittedForApproval(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderSubmittedForApproval(payload);


		[HttpPost, Route("OrderApproved")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderApproved(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderApproved(payload);


		[HttpPost, Route("OrderDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderDeclined(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderDeclined(payload);


		[HttpPost, Route("OrderSubmittedForYourApproval")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderSubmittedForYourApproval(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderSubmittedForYourApproval(payload);


		[HttpPost, Route("OrderSubmittedForYourApprovalHasBeenApproved")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderSubmittedForYourApprovalHasBeenApproved(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderSubmittedForYourApprovalHasBeenApproved(payload);


		[HttpPost, Route("OrderSubmittedForYourApprovalHasBeenDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderSubmittedForYourApprovalHasBeenDeclined(OrderMessageSenderPayloadWithXp payload)
			=> await _command.OrderSubmittedForYourApprovalHasBeenDeclined(payload);


		[HttpPost, Route("ShipmentCreated")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task ShipmentCreated(ShipmentCreatedMessageSenderPayloadWithXp payload)
			=> await _command.ShipmentCreated(payload);


		[HttpPost, Route("OrderReturnSubmitted")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnSubmitted(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnSubmitted(payload);


		[HttpPost, Route("OrderReturnSubmittedForApproval")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnSubmittedForApproval(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnSubmittedForApproval(payload);


		[HttpPost, Route("OrderReturnApproved")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnApproved(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnApproved(payload);


		[HttpPost, Route("OrderReturnDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnDeclined(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnDeclined(payload);


		[HttpPost, Route("OrderReturnSubmittedForYourApproval")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnSubmittedForYourApproval(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnSubmittedForYourApproval(payload);


		[HttpPost, Route("OrderReturnSubmittedForYourApprovalHasBeenDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnSubmittedForYourApprovalHasBeenApproved(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnSubmittedForYourApprovalHasBeenApproved(payload);


		[HttpPost, Route("OrderReturnSubmittedForYourApprovalHasBeenDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnSubmittedForYourApprovalHasBeenDeclined(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnSubmittedForYourApprovalHasBeenDeclined(payload);


		[HttpPost, Route("OrderReturnSubmittedForYourApprovalHasBeenDeclined")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task OrderReturnCompleted(OrderReturnMessageSenderPayloadWithXp payload)
			=> await _command.OrderReturnCompleted(payload);
	}
}
