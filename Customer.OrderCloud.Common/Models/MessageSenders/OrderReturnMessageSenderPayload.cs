using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models.MessageSenders
{
	/// <summary>
	/// Used for message sender types OrderReturnSubmitted, OrderReturnSubmittedForApproval, OrderReturnApproved, OrderReturnDeclined, OrderReturnSubmittedForYourApproval, OrderReturnSubmittedForYourApprovalHasBeenApproved, OrderReturnSubmittedForYourApprovalHasBeenDeclined, OrderReturnCompleted 
	/// </summary>
	public class OrderReturnMessageSenderPayload<TMessageSenderXp, TUser, TOrder, TOrderApproval, TLineItem, TProduct, TOrderReturn> : MessageSenderPayload<TMessageSenderXp, TUser>
		where TUser : User
		where TOrder : Order
		where TOrderApproval : OrderApproval
		where TLineItem : LineItem
		where TProduct : Product
		where TOrderReturn : OrderReturn
	{
		public OrderReturnMessageSenderEventBody<TOrder, TOrderApproval, TLineItem, TProduct, TOrderReturn> EventBody { get; set; }
	}

	/// <summary>
	/// Used for message sender types OrderReturnSubmitted, OrderReturnSubmittedForApproval, OrderReturnApproved, OrderReturnDeclined, OrderReturnSubmittedForYourApproval, OrderReturnSubmittedForYourApprovalHasBeenApproved, OrderReturnSubmittedForYourApprovalHasBeenDeclined, OrderReturnCompleted 
	/// </summary>
	public class OrderReturnMessageSenderEventBody<TOrder, TOrderApproval, TLineItem, TProduct, TOrderReturn>
		where TOrder : Order
		where TOrderApproval : OrderApproval
		where TLineItem : LineItem
		where TProduct : Product
		where TOrderReturn : OrderReturn
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
		/// The order return that was submitted
		/// </summary>
		public TOrderReturn OrderReturn { get; set; }
	}

}
