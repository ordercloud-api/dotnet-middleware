using Catalyst.Common;
using Customer.OrderCloud.Common.Models;
using OrderCloud.Catalyst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customer.OrderCloud.Common.Models.Xp;
using Customer.OrderCloud.Common.Models.MessageSenders;

namespace Customer.OrderCloud.Common.Commands
{
	public interface ISendEmailMapperCommand
	{
		Task SendSetPasswordEmail(SetPasswordMessageSenderPayloadWithXp payload);
		Task SendOrderEmail(OrderMessageSenderPayloadWithXp payload);
		Task SendShipmentCreatedEmail(ShipmentCreatedMessageSenderPayloadWithXp payload);
		Task SendOrderReturnEmail(OrderReturnMessageSenderPayloadWithXp payload);
	}

	public class SendEmailMapperCommand : ISendEmailMapperCommand
	{
		private readonly ISingleEmailSender _sender;

		public SendEmailMapperCommand(ISingleEmailSender sender)
		{
			_sender = sender;
		}

		/// <summary>
		/// https://ordercloud.io/knowledge-base/message-senders#variables-for-forgottenpassword-and-newuserinvitation
		/// </summary>
		public async Task SendSetPasswordEmail(SetPasswordMessageSenderPayloadWithXp payload)
		{
			var templateData = new
			{
				username = payload.EventBody.Username,
				passwordtoken = payload.EventBody.PasswordRenewalAccessToken,
				passwordverification = payload.EventBody.PasswordRenewalVerificationCode,
				passwordrenewalurl = payload.EventBody.PasswordRenewalUrl
			};
			await SendEmail(payload, templateData);
		}


		/// <summary>
		/// https://ordercloud.io/knowledge-base/message-senders#variables-for-order-emails
		/// </summary>
		public async Task SendOrderEmail(OrderMessageSenderPayloadWithXp payload)
		{
			var templateData = new
			{
				firstname = payload.EventBody.Order.FromUser.FirstName,
				lastname = payload.EventBody.Order.FromUser.LastName,
				orderid = payload.EventBody.Order.ID,
				datesubmitted = payload.EventBody.Order.DateSubmitted.ToString(),
				subtotal = payload.EventBody.Order.Subtotal.ToString(),
				tax = payload.EventBody.Order.TaxCost.ToString(),
				shipping = payload.EventBody.Order.ShippingCost.ToString(),
				total = payload.EventBody.Order.Total.ToString(),
				lineitemcount = payload.EventBody.Order.LineItemCount.ToString(),
				products = payload.EventBody.LineItems.Select(li =>
				{
					return new
					{
						cost = li.LineTotal.ToString(),
						quantity = li.Quantity.ToString(),
						productdesc = li.Product.Description,
						productid = li.Product.ID,
						productname = li.Product.Name,
						shiptoname = li.ShippingAddress.FirstName + " " + li.ShippingAddress.LastName,
						shiptostreet1 = li.ShippingAddress.Street1,
						shiptostreet2 = li.ShippingAddress.Street2,
						shiptocity = li.ShippingAddress.City,
						shiptostate = li.ShippingAddress.State,
						shiptopostalcode = li.ShippingAddress.Zip,
						shiptocountry = li.ShippingAddress.Country,
					};
				}),
				approvals = payload.EventBody.Approvals.Select(ap =>
				{
					return new
					{
						approvinggroupid = ap.ApprovingGroupID,
						status = ap.Status,
						datecreated = ap.DateCreated,
						datecompleted = ap.DateCompleted,
						approverid = ap.Approver.ID,
						approveremail = ap.Approver.Email,
						approverfirstname = ap.Approver.FirstName,
						approverlastname = ap.Approver.LastName,
						approverusername = ap.Approver.Username,
						approverphone = ap.Approver.Phone
					};
				}),
			};
			await SendEmail(payload, templateData);
		}


		/// <summary>
		/// https://ordercloud.io/knowledge-base/message-senders#variables-for-shipmentcreated
		/// </summary>
		public async Task SendShipmentCreatedEmail(ShipmentCreatedMessageSenderPayloadWithXp payload)
		{
			var templateData = new
			{
				shipmentid = payload.EventBody.Shipment.ID,
				shipmenttrackingnumber = payload.EventBody.Shipment.TrackingNumber,
				shipper = payload.EventBody.Shipment.Shipper,
				dateshipped = payload.EventBody.Shipment.DateShipped.ToString(),
				toaddressid = payload.EventBody.Shipment.ToAddress.ID,
				toaddresscompany = payload.EventBody.Shipment.ToAddress.CompanyName,
				toaddressfirstname = payload.EventBody.Shipment.ToAddress.FirstName,
				toaddresslastname = payload.EventBody.Shipment.ToAddress.LastName,
				toaddressstreet1 = payload.EventBody.Shipment.ToAddress.Street1,
				toaddressstreet2 = payload.EventBody.Shipment.ToAddress.Street2,
				toaddresscity =	payload.EventBody.Shipment.ToAddress.City,
				toaddressstate = payload.EventBody.Shipment.ToAddress.State,
				toaddresscountry = payload.EventBody.Shipment.ToAddress.Country,
				toaddresspostalcode = payload.EventBody.Shipment.ToAddress.Zip,
				toaddressname = payload.EventBody.Shipment.ToAddress.AddressName,
				shipmentitems = payload.EventBody.ShipmentItems.Select(shipmentItem =>
				{
					var lineItem = payload.EventBody.LineItems.FirstOrDefault(li => li.ID == shipmentItem.LineItemID);
					return new
					{
						cost = lineItem?.LineTotal.ToString() ?? "",
						quantityshipped = shipmentItem.QuantityShipped,
						productdesc = shipmentItem.Product.Description,
						productid = shipmentItem.Product.ID,
						productname = shipmentItem.Product.Name
					};
				})
			};
			await SendEmail(payload, templateData);
		}

		/// <summary>
		/// https://ordercloud.io/knowledge-base/message-senders#variables-for-orderreturn
		/// </summary>
		public async Task SendOrderReturnEmail(OrderReturnMessageSenderPayloadWithXp payload)
		{
			var templateData = new
			{
				orderreturnid = payload.EventBody.OrderReturn.ID,
				refundamount = payload.EventBody.OrderReturn.RefundAmount,
				returnitems = payload.EventBody.OrderReturn.ItemsToReturn.Select(item =>
				{
					return new
					{
						LineItemID = item.LineItemID,
						Quantity = item.Quantity,
						RefundAmount = item.RefundAmount
					};
				})
			};
			await SendEmail(payload, templateData);
		}

		private async Task SendEmail(MessageSenderPayload payload, object templateData)
		{
			var to = payload.Recipient.Email;
			var config = GetConfigForThisMessageType(payload);
			// Email Builder supports many options.
			var message = EmailBuilder.BuildTemplateEmail(to, config.FromEmail, config.Subject, config.TemplateName, templateData);
			await _sender.SendSingleEmailAsync(message);

			// TODO - drop onto a queue for async processing
		}

		private MessageTypeConfig GetConfigForThisMessageType(OrderReturnMessageSenderPayloadWithXp payload)
		{
			return (payload.ConfigData as Models.Xp.MessageSenderXp).MessageTypeConfig.FirstOrDefault(c => c.MessageType == payload.MessageType.ToString());
		}
	}
}
