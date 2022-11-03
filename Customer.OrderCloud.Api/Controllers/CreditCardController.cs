using Customer.OrderCloud.Common.Commands;
using Customer.OrderCloud.Common.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Api.Controllers
{
	[Route("api/me/credit-cards")]
	public class CreditCardController : CatalystController
	{
		private readonly ICreditCardCommand _creditCardCommand;
		private readonly IOrderCloudClient _oc;

		public CreditCardController(CreditCardCommand creditCardCommand, IOrderCloudClient oc)
		{
			_creditCardCommand = creditCardCommand;
			_oc = oc;
		}

		[HttpGet, Route("")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<List<PCISafeCardDetails>> ListSavedCreditCardsAsync()
		{
			var shopper = await _oc.Me.GetAsync<MeUserWithXp>(UserContext.AccessToken);
			return await _creditCardCommand.ListSavedCardsAsync(shopper);
		}

		[HttpPost, Route("")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<PCISafeCardDetails> CreateSavedCardAsync([FromBody] PCISafeCardDetails card)
		{
			var shopper = await _oc.Me.GetAsync<MeUserWithXp>(UserContext.AccessToken);
			return await _creditCardCommand.CreateSavedCardAsync(shopper, card);
		}

		[HttpGet, Route("{cardID}")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task<PCISafeCardDetails> GetSavedCardAsync(string cardID)
		{
			var shopper = await _oc.Me.GetAsync<MeUserWithXp>(UserContext.AccessToken);
			return await _creditCardCommand.GetSavedCardAsync(shopper, cardID);
		}

		[HttpDelete, Route("{cardID}")]
		[OrderCloudUserAuth(ApiRole.Shopper), UserTypeRestrictedTo(CommerceRole.Buyer)]
		public async Task DeleteSavedCardASync(string cardID)
		{
			var shopper = await _oc.Me.GetAsync<MeUserWithXp>(UserContext.AccessToken);
			await _creditCardCommand.DeleteSavedCardASync(shopper, cardID);
		}
	}
}
