using Customer.OrderCloud.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
	public interface IOrderReturnCommand
	{
		Task<OrderReturnResponse> CalculateOrderReturnRefundAsync(OrderReturnIEPayloadWithXp payload);
	}

	public class OrderReturnCommand
	{
		public async Task<OrderReturnResponse> CalculateOrderReturnRefundAsync(OrderReturnIEPayloadWithXp payload)
		{
			// Calculate how much the customer should be refunded on the whole order, and how much should be taken off each line item.

			return new OrderReturnResponse();
		}

		//Question - what OC event should trigger the refund in the tax and payment system?
	}
}
