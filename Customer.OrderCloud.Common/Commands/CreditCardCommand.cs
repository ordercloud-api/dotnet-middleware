using Customer.OrderCloud.Common.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
	public interface ICreditCardCommand
	{
		Task<List<SavedCreditCard>> ListSavedCardsAsync(MeUserWithXp shopper);
		Task<SavedCreditCard> GetSavedCardAsync(MeUserWithXp shopper, string cardID);
		Task<SavedCreditCard> CreateSavedCardAsync(MeUserWithXp shopper, PCISafeCardDetails card);
	}

	public class CreditCardCommand : ICreditCardCommand
	{
		private readonly ICreditCardSaver _creditCardSaver;
		private readonly IOrderCloudClient _oc;
		public CreditCardCommand(ICreditCardSaver creditCardSaver, IOrderCloudClient oc)
		{
			_creditCardSaver = creditCardSaver;
			_oc = oc;
		}

		public async Task<SavedCreditCard> GetSavedCardAsync(MeUserWithXp shopper, string cardID)
		{
			var customerID = shopper?.xp?.PaymentProcessorCustomerID;
			var card = await _creditCardSaver.GetSavedCardAsync(customerID, cardID);
			return card;
		}


		public async Task<SavedCreditCard> CreateSavedCardAsync(MeUserWithXp shopper, PCISafeCardDetails card)
		{
			var customerRecordExists = shopper?.xp?.PaymentProcessorCustomerID != null;
			var paymentProcessorCustomerID = $"{shopper.Buyer.ID}-{shopper.ID}";
			var customer = new PaymentSystemCustomer()
			{
				ID = shopper?.xp?.PaymentProcessorCustomerID ?? paymentProcessorCustomerID,
				Email = shopper.Email,
				FirstName = shopper.FirstName,
				LastName = shopper.LastName,
				CustomerRecordExists = customerRecordExists,
			};
			var savedCard = await _creditCardSaver.CreateSavedCardAsync(customer, card);
			if (!customerRecordExists)
			{
				var patch = new PartialUser<MeUserXp>() { xp = new { PaymentProcessorCustomerID = paymentProcessorCustomerID } };
				await _oc.Users.PatchAsync(shopper.Buyer.ID, shopper.ID, patch);
			}
			return savedCard;
		}

		public async Task<List<SavedCreditCard>> ListSavedCardsAsync(MeUserWithXp shopper)
		{
			if (shopper?.xp?.PaymentProcessorCustomerID == null)
			{
				return new List<SavedCreditCard>();
			}
			var savedCards = await _creditCardSaver.ListSavedCardsAsync(shopper.xp.PaymentProcessorCustomerID);
			return savedCards;
		}
	}
}
