using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models.MessageSenders
{
	/// <summary>
	/// Used for the ShipmentCreated message sender type
	/// </summary>
	public class ShipmentCreatedMessageSenderPayload<TMessageSenderXp, TUser, TOrder, TOrderApproval, TLineItem, TProduct, TShipment, TShipmentItem> : MessageSenderPayload<TMessageSenderXp, TUser>
		where TUser : User
		where TOrder : Order
		where TOrderApproval : OrderApproval
		where TLineItem : LineItem
		where TProduct : Product
		where TShipment : Shipment
		where TShipmentItem : ShipmentItem
	{
		public ShipmentCreatedMessageSenderEventBody<TOrder, TOrderApproval, TLineItem, TProduct, TShipment, TShipmentItem> EventBody { get; set; }
	}

	public class ShipmentCreatedMessageSenderEventBody<TOrder, TOrderApproval, TLineItem, TProduct, TShipment, TShipmentItem>
		where TOrder : Order
		where TOrderApproval : OrderApproval
		where TLineItem : LineItem
		where TProduct : Product
		where TShipment : Shipment
		where TShipmentItem : ShipmentItem
	{
		/// <summary>
		/// The order that was submitted
		/// </summary>
		public TOrder Order { get; set; }
		/// <summary>
		/// The array of order approvals for the order
		/// </summary>
		public List<TOrderApproval> Approvals { get; set; }
		/// <summary>
		/// The array of line items for the orders
		/// </summary>
		public List<TLineItem> LineItems { get; set; }
		/// <summary>
		/// The array of products for the order
		/// </summary>
		public List<TProduct> Products { get; set; }
		/// <summary>
		/// The shipment for the order
		/// </summary>
		public TShipment Shipment { get; set; }
		/// <summary>
		/// The array of shipment items for the shipment
		/// </summary>
		public List<TShipmentItem> ShipmentItems { get; set; }
	}
}
