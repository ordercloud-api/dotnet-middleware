using Catalyst.Common;
using Customer.OrderCloud.Common.Commands;
using Customer.OrderCloud.Common.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Catalyst.Api.Controllers
{
	[Route("api")]
	public class CheckoutController : CatalystController
	{
		private readonly ICheckoutCommand _checkoutCommand;
		private readonly IOrderCloudClient _oc;
		public CheckoutController(ICheckoutCommand checkoutCommand, IOrderCloudClient oc)
		{
			_checkoutCommand = checkoutCommand;
			_oc = oc;
		}

		// Hit from Storefront Client
		[HttpPut, Route("order/{orderID}/card-payment")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<PaymentWithXp> SetCreditCardPaymentAsync(string orderID, CreditCardPayment payment)
		{
			var shopper = await _oc.Me.GetAsync<MeUserWithXp>(UserContext.AccessToken);
			return await _checkoutCommand.SetCreditCardPaymentAsync(orderID, shopper, payment);
		}

		// Hit from Storefront Client
		[HttpPost, Route("order/{orderID}/submit")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<OrderConfirmation> SubmitOrderAsync(string orderID) =>
			await _checkoutCommand.SubmitOrderAsync(orderID, UserContext);
	}
}
