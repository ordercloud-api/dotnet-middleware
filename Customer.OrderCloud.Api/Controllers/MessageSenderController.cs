using OrderCloud.Catalyst;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Customer.OrderCloud.Common.Models;
using Customer.OrderCloud.Common.Commands;
using Customer.OrderCloud.Common.Models.MessageSenders;

namespace Customer.OrderCloud.Api.Controllers
{
	[Route("api/integrationevents")]
	public class MessageSenderController : CatalystController
	{
		private readonly IMessageSenderCommand _command;

		public MessageSenderController(IMessageSenderCommand command)
		{
			_command = command;
		}

		[HttpPost, Route("NewUserInvitation")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task NewUserInvitation(SetPasswordMessageSenderPayload payload)
		{

		}

		[HttpPost, Route("ForgottenPassword")]
		[OrderCloudWebhookAuth] // Security feature to verifiy request came from Ordercloud.
		public async Task ForgottenPassword(SetPasswordMessageSenderPayload payload)
		{

		}

	}
}
