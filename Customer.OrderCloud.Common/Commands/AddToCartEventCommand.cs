using Customer.OrderCloud.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
	public interface IAddToCartEventCommand
	{
		Task<AddToCartResponseWithXp> GetProductWithUnitPriceAsync(AddToCartIEPayloadWithXp payload);
	}

	public class AddToCartEventCommand : IAddToCartEventCommand
	{
		public AddToCartEventCommand()
		{

		}

		public async Task<AddToCartResponseWithXp> GetProductWithUnitPriceAsync(AddToCartIEPayloadWithXp payload)
		{
			// Get Product details from External system. Calculate unit price if not already provided.
			return new AddToCartResponseWithXp();
		}
	}
}
