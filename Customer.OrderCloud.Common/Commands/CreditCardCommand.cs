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
		Task DeleteSavedCardASync(MeUserWithXp shopper, string cardID);
		Task<PaymentWithXp> AuthorizeCardPayment(OrderWorksheetWithXp worksheet, PaymentWithXp payment);
		Task<PaymentWithXp> CaptureCardPayment(string orderID, PaymentWithXp payment);
		Task<PaymentWithXp> VoidOrRefundCardPayment(string orderID, PaymentWithXp payment);
	}

	public class CreditCardCommand : ICreditCardCommand
	{
		private readonly ICreditCardSaver _creditCardSaver;
		private readonly ICreditCardProcessor _creditCardProcessor;
		private readonly IOrderCloudClient _oc;
		public CreditCardCommand(ICreditCardSaver creditCardSaver, IOrderCloudClient oc, ICreditCardProcessor creditCardProcessor)
		{
			_creditCardProcessor = creditCardProcessor;
			_creditCardSaver = creditCardSaver;
			_oc = oc;
		}

		public async Task<List<PCISafeCardDetails>> ListSavedCardsAsync(MeUserWithXp shopper)
		{
			var customerID = shopper?.xp?.PaymentProcessorCustomerID;
			if (customerID == null)
			{
				return new List<PCISafeCardDetails>();
			}
			var savedCards = await _creditCardSaver.ListSavedCardsAsync(shopper.xp.PaymentProcessorCustomerID);
			return savedCards;
		}

		public async Task<PCISafeCardDetails> GetSavedCardAsync(MeUserWithXp shopper, string cardID)
		{
			var customerID = shopper?.xp?.PaymentProcessorCustomerID;
			if (customerID == null)
			{
				return null;
			}
			var card = await _creditCardSaver.GetSavedCardAsync(customerID, cardID);
			return card;
		}

		public async Task<PCISafeCardDetails> CreateSavedCardAsync(MeUserWithXp shopper, PCISafeCardDetails card)
		{
			var customerID = shopper?.xp?.PaymentProcessorCustomerID;
			var customer = new PaymentSystemCustomer()
			{
				ID = shopper?.xp?.PaymentProcessorCustomerID, // cannot assume customer ID is set-able
				Email = shopper.Email,
				FirstName = shopper.FirstName,
				LastName = shopper.LastName,
				CustomerAlreadyExists = customerID != null,
			};
			var savedCard = await _creditCardSaver.CreateSavedCardAsync(customer, card);
			if (!customer.CustomerAlreadyExists)
			{
				var patch = new PartialUser<MeUserWithXp>() { xp = new { PaymentProcessorCustomerID = savedCard.CustomerID } };
				await _oc.Users.PatchAsync(shopper.Buyer.ID, shopper.ID, patch);
			}
			return savedCard.Card;
		}

		public async Task DeleteSavedCardASync(MeUserWithXp shopper, string cardID)
		{
			var customerID = shopper?.xp?.PaymentProcessorCustomerID;
			await _creditCardSaver.DeleteSavedCardAsync(customerID, cardID);
		}

		public async Task<PaymentWithXp> AuthorizeCardPayment(OrderWorksheetWithXp worksheet, PaymentWithXp payment)
		{
			//TODO - probably some checks 
			var authorizeRequest = new AuthorizeCCTransaction()
			{
				OrderID = worksheet.Order.ID,
				Amount = worksheet.Order.Total,
				Currency = worksheet.Order.Currency,
				AddressVerification = worksheet.Order.BillingAddress,
				CustomerIPAddress = "",
			};
			var payWithSavedCard = payment?.xp?.SafeCardDetails?.SavedCardID != null;
			if (payWithSavedCard)
			{
				authorizeRequest.SavedCardID = payment.xp.SafeCardDetails.SavedCardID;
				authorizeRequest.ProcessorCustomerID = worksheet.Order.FromUser.xp.PaymentProcessorCustomerID;
			}
			else
			{
				authorizeRequest.CardToken = payment?.xp?.SafeCardDetails?.Token;
			}

			var authorizationResult = await _creditCardProcessor.AuthorizeOnlyAsync(authorizeRequest);

			Require.That(authorizationResult.Succeeded, MyErrorCodes.Payment.AuthorizationFailed, authorizationResult);

			await _oc.Payments.PatchAsync<PaymentWithXp>(OrderDirection.All, worksheet.Order.ID, payment.ID, new PartialPayment { Accepted = true, Amount = authorizeRequest.Amount });
			var updatedPayment = await CreatePaymentTransaction(worksheet.Order.ID, payment, PaymentTransactionType.Authorization, authorizationResult);
			return updatedPayment;
		}

		public async Task<PaymentWithXp> CaptureCardPayment(string orderID, PaymentWithXp payment)
		{
			var authorizationTransaction = GetLastSuccessfulTransactionOfType(PaymentTransactionType.Authorization, payment);
			Require.That(authorizationTransaction != null, MyErrorCodes.Payment.CannotCapture, payment);
			var captureResult = await _creditCardProcessor.CapturePriorAuthorizationAsync(new FollowUpCCTransaction()
			{
				Amount = payment.Amount ?? 0,
				TransactionID = authorizationTransaction.ID
			});
			var updatedPayment = await CreatePaymentTransaction(orderID, payment, PaymentTransactionType.Capture, captureResult);

			return updatedPayment;
		}

		public async Task<PaymentWithXp> VoidOrRefundCardPayment(string orderID, PaymentWithXp payment)
		{
			var authorizationTransaction = GetLastSuccessfulTransactionOfType(PaymentTransactionType.Authorization, payment);
			if (authorizationTransaction == null)
			{
				return payment;
			}
			var isCaptured = payment.Transactions.Any(x => x.Type == PaymentTransactionType.Capture.ToString());
			Func<FollowUpCCTransaction, OCIntegrationConfig, Task<CCTransactionResult>> reverseMethod;
			if (isCaptured)
			{
				reverseMethod = _creditCardProcessor.RefundCaptureAsync;
			} else
			{
				reverseMethod = _creditCardProcessor.VoidAuthorizationAsync;
			}

			var reverseTransactionResult = await reverseMethod(new FollowUpCCTransaction()
			{
				TransactionID = authorizationTransaction.ID,
				Amount = authorizationTransaction.Amount ?? 0
			}, null);

			var type = isCaptured ? PaymentTransactionType.Refund : PaymentTransactionType.Void;

			var updatedPayment = await CreatePaymentTransaction(orderID, payment, type, reverseTransactionResult);
			return updatedPayment;
		}

		private async Task<PaymentWithXp> CreatePaymentTransaction(string orderID, PaymentWithXp payment, PaymentTransactionType type, CCTransactionResult transaction)
		{
			var updatedPayment = await _oc.Payments.CreateTransactionAsync<PaymentWithXp>(OrderDirection.All, orderID, payment.ID, new PaymentTransactionWithXp()
			{
				ID = transaction.TransactionID,
				Amount = payment.Amount,
				DateExecuted = DateTime.Now,
				ResultCode = transaction.AuthorizationCode,
				ResultMessage = transaction.Message,
				Succeeded = transaction.Succeeded,
				Type = type.ToString(),
				xp = new PaymentTransactionXp
				{
					TransactionDetails = transaction,
				}
			});
			return updatedPayment;
		}

		private PaymentTransactionWithXp GetLastSuccessfulTransactionOfType(PaymentTransactionType type, PaymentWithXp payment)
		{
			var authorizationTransaction = payment.Transactions
				.Where(x => x.Type == type.ToString())
				.OrderBy(x => x.DateExecuted)
				.LastOrDefault(t => t.Succeeded);
			return authorizationTransaction;
		}
	}
}
