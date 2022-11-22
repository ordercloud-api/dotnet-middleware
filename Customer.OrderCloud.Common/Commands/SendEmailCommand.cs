using Customer.OrderCloud.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
	public interface ISendEmailCommand
	{
		Task NewUserInvitation(SetPasswordMessageSenderPayloadWithXp payload);
		Task ForgottenPassword(SetPasswordMessageSenderPayloadWithXp payload);
		Task OrderSubmitted(OrderMessageSenderPayloadWithXp payload);
		Task OrderSubmittedForApproval(OrderMessageSenderPayloadWithXp payload);
		Task OrderApproved(OrderMessageSenderPayloadWithXp payload);
		Task OrderDeclined(OrderMessageSenderPayloadWithXp payload);
		Task OrderSubmittedForYourApproval(OrderMessageSenderPayloadWithXp payload);
		Task OrderSubmittedForYourApprovalHasBeenApproved(OrderMessageSenderPayloadWithXp payload);
		Task OrderSubmittedForYourApprovalHasBeenDeclined(OrderMessageSenderPayloadWithXp payload);
		Task ShipmentCreated(ShipmentCreatedMessageSenderPayloadWithXp payload);
		Task OrderReturnSubmitted(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnSubmittedForApproval(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnApproved(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnDeclined(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnSubmittedForYourApproval(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnSubmittedForYourApprovalHasBeenApproved(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnSubmittedForYourApprovalHasBeenDeclined(OrderReturnMessageSenderPayloadWithXp payload);
		Task OrderReturnCompleted(OrderReturnMessageSenderPayloadWithXp payload);
	}

	public class SendEmailCommand : ISendEmailCommand
	{
		public async Task NewUserInvitation(SetPasswordMessageSenderPayloadWithXp payload)
		{

		}

		public async Task ForgottenPassword(SetPasswordMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderSubmitted(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderSubmittedForApproval(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderApproved(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderDeclined(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderSubmittedForYourApproval(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderSubmittedForYourApprovalHasBeenApproved(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderSubmittedForYourApprovalHasBeenDeclined(OrderMessageSenderPayloadWithXp payload)
		{

		}

		public async Task ShipmentCreated(ShipmentCreatedMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnSubmitted(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnSubmittedForApproval(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnApproved(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnDeclined(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnSubmittedForYourApproval(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnSubmittedForYourApprovalHasBeenApproved(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnSubmittedForYourApprovalHasBeenDeclined(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}

		public async Task OrderReturnCompleted(OrderReturnMessageSenderPayloadWithXp payload)
		{

		}
	}
}
