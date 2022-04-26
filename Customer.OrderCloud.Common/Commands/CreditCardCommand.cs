using Customer.OrderCloud.Common.Models;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
	public interface ICreditCardCommand
	{
		Task<List<PCISafeCardDetails>> ListSavedCardsAsync(MeUserWithXp shopper);
		Task<PCISafeCardDetails> GetSavedCardAsync(MeUserWithXp shopper, string cardID);
		Task<PCISafeCardDetails> CreateSavedCardAsync(MeUserWithXp shopper, PCISafeCardDetails card);
		Task<PaymentWithXp> AuthorizeCardPayment(OrderWorksheetWithXp worksheet, PaymentWithXp payment);
		Task<PaymentWithXp> VoidCardPayment(string orderID, PaymentWithXp payment);

	}

	public class CreditCardCommand : ICreditCardCommand
	{
		private readonly ICreditCardSaver _creditCardSaver;
		private readonly ICreditCardProcessor _creditCardProcessor;
		private readonly IOrderCloudClient _oc;
		public CreditCardCommand(ICreditCardSaver creditCardSaver, IOrderCloudClient oc)
		{
			_creditCardSaver = creditCardSaver;
			_oc = oc;
		}

		public async Task<PCISafeCardDetails> GetSavedCardAsync(MeUserWithXp shopper, string cardID)
		{
			var customerID = shopper?.xp?.PaymentProcessorCustomerID;
			var card = await _creditCardSaver.GetSavedCardAsync(customerID, cardID);
			return card;
		}


		public async Task<PCISafeCardDetails> CreateSavedCardAsync(MeUserWithXp shopper, PCISafeCardDetails card)
		{
			var customerRecordExists = shopper?.xp?.PaymentProcessorCustomerID != null;
			var paymentProcessorCustomerID = $"{shopper.Buyer.ID}-{shopper.ID}";
			var customer = new PaymentSystemCustomer()
			{
				ID = shopper?.xp?.PaymentProcessorCustomerID ?? paymentProcessorCustomerID,
				Email = shopper.Email,
				FirstName = shopper.FirstName,
				LastName = shopper.LastName,
				CustomerAlreadyExists = customerRecordExists,
			};
			var savedCard = await _creditCardSaver.CreateSavedCardAsync(customer, card);
			if (!customerRecordExists)
			{
				var patch = new PartialUser<MeUserWithXp>() { xp = new { PaymentProcessorCustomerID = paymentProcessorCustomerID } };
				await _oc.Users.PatchAsync(shopper.Buyer.ID, shopper.ID, patch);
			}
			return savedCard;
		}

		public async Task<List<PCISafeCardDetails>> ListSavedCardsAsync(MeUserWithXp shopper)
		{
			if (shopper?.xp?.PaymentProcessorCustomerID == null)
			{
				return new List<PCISafeCardDetails>();
			}
			var savedCards = await _creditCardSaver.ListSavedCardsAsync(shopper.xp.PaymentProcessorCustomerID);
			return savedCards;
		}

		public async Task<PaymentWithXp> AuthorizeCardPayment(OrderWorksheetWithXp worksheet, PaymentWithXp payment)
		{
			var authorizeRequest = new AuthorizeCCTransaction()
			{
				OrderID = worksheet.Order.ID,
				Amount = worksheet.Order.Total,
				Currency = worksheet.Order.Currency,
				AddressVerification = worksheet.Order.BillingAddress,
				CustomerIPAddress = "",
			};
			var payWithSavedCard = payment?.xp?.SafeCardDetails?.ProcessorSavedCardID != null;
			if (payWithSavedCard)
			{
				authorizeRequest.SavedCardID = payment.xp.SafeCardDetails.ProcessorSavedCardID;
				authorizeRequest.ProcessorCustomerID = worksheet.Order.FromUser.xp.PaymentProcessorCustomerID;
			}
			else
			{
				authorizeRequest.CardToken = payment?.xp?.SafeCardDetails?.Token;
			}

			var authorizationResult = await _creditCardProcessor.AuthorizeOnlyAsync(authorizeRequest);
			if (authorizationResult.Succeeded)
			{
				await _oc.Payments.PatchAsync<PaymentWithXp>(OrderDirection.All, worksheet.Order.ID, payment.ID, new PartialPayment { Accepted = true, Amount = authorizeRequest.Amount });
				var updatedPayment = await _oc.Payments.CreateTransactionAsync<PaymentWithXp>(OrderDirection.All, worksheet.Order.ID, payment.ID, new PaymentTransactionWithXp()
				{
					ID = authorizationResult.TransactionID,
					Amount = payment.Amount,
					DateExecuted = DateTime.Now,
					ResultCode = authorizationResult.AuthorizationCode,
					ResultMessage = authorizationResult.Message,
					Succeeded = authorizationResult.Succeeded,
					Type = PaymentTransactionType.Authorization.ToString(),
					xp = new PaymentTransactionXp
					{
						TransactionDetails = authorizationResult,
					}
				});
				return updatedPayment;
			}
			else
			{
				throw new CatalystBaseException(new ApiError()
				{
					Data = authorizationResult,
					Message = authorizationResult.Message,
					ErrorCode = $"Payment.AuthorizeDidNotSucceed"
				});
			}
		}

		public async Task<PaymentWithXp> VoidCardPayment(string orderID, PaymentWithXp payment)
		{
			var transaction = payment.Transactions
				.Where(x => x.Type == PaymentTransactionType.Authorization.ToString())
				.OrderBy(x => x.DateExecuted)
				.LastOrDefault(t => t.Succeeded);

			// What if void fails? Put it in a support queue for manual intervention.
			var voidResult = await _creditCardProcessor.VoidAuthorizationAsync(new FollowUpCCTransaction()
			{
				TransactionID = transaction.ID,
				Amount = transaction.Amount ?? 0
			});

			var updatedPayment = await _oc.Payments.CreateTransactionAsync<PaymentWithXp>(OrderDirection.All, orderID, payment.ID, new PaymentTransactionWithXp()
			{
				ID = voidResult.TransactionID,
				Amount = payment.Amount,
				DateExecuted = DateTime.Now,
				ResultCode = voidResult.AuthorizationCode,
				ResultMessage = voidResult.Message,
				Succeeded = voidResult.Succeeded,
				Type = PaymentTransactionType.Void.ToString(),
				xp = new PaymentTransactionXp
				{
					TransactionDetails = voidResult,
				}
			});
			return updatedPayment;
		}

	}
}
