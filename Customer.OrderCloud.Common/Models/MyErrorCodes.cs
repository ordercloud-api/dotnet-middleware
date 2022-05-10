using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
	public static class MyErrorCodes
	{
		private static IDictionary<string, ErrorCode> All { get; } = new Dictionary<string, ErrorCode>
		{
			{ "Payment.DetailsMissing", new ErrorCode("Payment.DetailsMissing", "Create credit card payment must include either CardDetails or SavedCardID.", HttpStatusCode.BadRequest) },
			{ "Payment.AuthorizationFailed", new ErrorCode("Payment.AuthorizationFailed", "Customer's payment could not be authorized successfully.", HttpStatusCode.BadRequest) },
			{ "Payment.CannotCapture", new ErrorCode("Payment.CannotCapture", "Cannot capture this payment because it has not yet been authorized.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.AlreadySubmitted", new ErrorCode("OrderSubmit.AlreadySubmitted", "Order has already been submitted.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.OrderCalculateError", new ErrorCode("OrderSubmit.OrderCalculateError", "A problem occurred during Order Calculation.  Please go back to the cart and try to checkout again.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.MissingShippingSelections", new ErrorCode("OrderSubmit.MissingShippingSelections", "All shipments on an order must have a selection.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.MissingPayment", new ErrorCode("OrderSubmit.MissingPayment", "Order must include credit card payment details.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.InvalidProducts", new ErrorCode("OrderSubmit.InvalidProducts", "Order contains line items for products that are inactive.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.OrderCloudValidationError", new ErrorCode("OrderSubmit.OrderCloudValidationError", "Failed ordercloud validation, see Data for details.", HttpStatusCode.BadRequest) },
			{ "OrderSubmit.PricesHaveChanged", new ErrorCode("OrderSubmit.PricesHaveChanged", "Did not submit order because prices have changed. Please review latest order details and re-submit.", HttpStatusCode.BadRequest) },
		};

		public static class Payment
		{
			public static readonly ErrorCode DetailsMissing = All["Payment.DetailsMissing"];
			public static readonly ErrorCode<CCTransactionResult> AuthorizationFailed = All["Payment.AuthorizationFailed"] as ErrorCode<CCTransactionResult>;
			public static readonly ErrorCode<PaymentWithXp> CannotCapture = All["Payment.CannotCapture"] as ErrorCode<PaymentWithXp>;
		}

		public static class OrderSubmit
		{
			public static readonly ErrorCode AlreadySubmitted = All["OrderSubmit.AlreadySubmitted"];
			public static readonly ErrorCode OrderCalculateError = All["OrderSubmit.OrderCalculateError"];
			public static readonly ErrorCode<IEnumerable<ShipEstimateWithXp>> MissingShippingSelections = All["OrderSubmit.MissingShippingSelections"] as ErrorCode<IEnumerable<ShipEstimateWithXp>>;
			public static readonly ErrorCode MissingPayment = All["OrderSubmit.MissingPayment"];
			public static readonly ErrorCode<List<LineItemWithXp>> InvalidProducts = All["OrderSubmit.InvalidProducts"] as ErrorCode<List<LineItemWithXp>>;
			public static readonly ErrorCode<IEnumerable<ApiError>> OrderCloudValidationError = All["OrderSubmit.OrderCloudValidationError"] as ErrorCode<IEnumerable<ApiError>>;
			public static readonly ErrorCode PricesHaveChanged = All["OrderSubmit.PricesHaveChanged"];
		}
	}
}
