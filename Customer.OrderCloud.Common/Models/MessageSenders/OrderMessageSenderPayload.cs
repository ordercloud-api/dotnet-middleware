using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models.MessageSenders
{
	/// <summary>
	/// Used for message sender types OrderSubmitted, OrderSubmittedForApproval, OrderApproved, OrderDeclined, OrderSubmittedForYourApproval, OrderSubmittedForYourApprovalHasBeenApproved, and OrderSubmittedForYourApprovalHasBeenDeclined.
	/// </summary>
	public class OrderMessageSenderPayload<TMessageSenderXp, TUser, TOrder, TOrderApproval, TLineItem, TProduct> : MessageSenderPayload<TMessageSenderXp, TUser>
		where TUser: User
		where TOrder : Order
		where TOrderApproval : OrderApproval
		where TLineItem : LineItem
		where TProduct : Product
	{
		public OrderMessageSenderEventBody<TOrder, TOrderApproval, TLineItem, TProduct> EventBody { get; set; }
	}

	/// <summary>
	/// Used for message sender types OrderSubmitted, OrderSubmittedForApproval, OrderApproved, OrderDeclined, OrderSubmittedForYourApproval, OrderSubmittedForYourApprovalHasBeenApproved, and OrderSubmittedForYourApprovalHasBeenDeclined.
	/// </summary>
	public class OrderMessageSenderEventBody<TOrder, TOrderApproval, TLineItem, TProduct>
		where TOrder : Order
		where TOrderApproval : OrderApproval
		where TLineItem : LineItem
		where TProduct : Product
	{
		/// <summary>
		/// The order that was submitted
		/// </summary>
		public TOrder Order { get; set; }
		/// <summary>
		/// The array of order approvals for the order
		/// </summary>
		public List<TOrderApproval> Approvals { get; set;}
		/// <summary>
		/// The array of line items for the orders
		/// </summary>
		public List<TLineItem> LineItems { get; set; }
		/// <summary>
		/// The array of products for the order
		/// </summary>
		public List<TProduct> Products { get; set; }
	}

}
