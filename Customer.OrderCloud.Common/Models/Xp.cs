using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Customer.OrderCloud.Common.Models
{
	public class OrderCalculatePayloadWithXp : OrderCalculatePayload<CheckoutConfig, OrderWithXp, LineItemWithXp, ShipEstimateResponseWithXp, OrderCalculateResponseWithXp, OrderSubmitResponseWithXp, OrderSubmitForApprovalResponseWithXp, OrderApprovedResponseWithXp> { }
	public class OrderWorksheetWithXp: OrderWorksheet<OrderWithXp, LineItemWithXp, ShipEstimateResponseWithXp, OrderCalculateResponseWithXp, OrderSubmitResponseWithXp, OrderSubmitForApprovalResponseWithXp, OrderApprovedResponseWithXp> { }
	public class OrderWithXp : Order<OrderXp, UserWithXp, BillingAddressWithXp> { }
	public class UserWithXp : User<UserXp> { }
	public class MeUserWithXp : MeUser<UserXp> { }
	public class BillingAddressWithXp: Address<BillingAddressXp> { }
	public class LineItemWithXp : LineItem<LineItemXp, LineItemProductWithXp, LineItemVariantWithXp, ShippingAddressWithXp, SupplierAddressWithXp> { }
	public class ProductWithXp : Product<ProductXp> { }
	public class LineItemProductWithXp : LineItemProduct<ProductXp> { }
	public class VariantWithXp : Variant<VariantXp> { }
	public class LineItemVariantWithXp : LineItemVariant<VariantXp> { }
	public class ShippingAddressWithXp : Address<ShippingAddressXp> { }
	public class SupplierAddressWithXp : Address<SupplierAddressXp> { }
	public class ShipEstimateResponseWithXp : ShipEstimateResponse<ShipEstimateResponseXp, ShipEstimateWithXp> { }
	public class ShipEstimateWithXp : ShipEstimate<ShipEstimateXp, ShipMethodWithXp> { }
	public class ShipMethodWithXp : ShipMethod<ShipMethodXp> { }
	public class OrderCalculateResponseWithXp: OrderCalculateResponse<OrderCalculateResponseXp> { }
	public class OrderSubmitResponseWithXp : OrderSubmitResponse<OrderSubmitResponseXp> { }
	public class OrderSubmitForApprovalResponseWithXp : OrderSubmitForApprovalResponse<OrderSubmitForApprovalResponseXp> { }
	public class OrderApprovedResponseWithXp : OrderApprovedResponse<OrderApprovedResponseXp> { }
	public class PaymentWithXp : Payment<PaymentXp, PaymentTransactionWithXp> { }
	public class PaymentTransactionWithXp : PaymentTransaction<PaymentTransactionXp> { }


	// Data configured in OrderCloud to be passed to all Checkout Integration Events as Payload.ConfigData
	public class CheckoutConfig
	{

	}

	public class OrderXp 
	{
		/// <summary>
		/// True if one of the post-submit processes failed
		/// </summary>
		public bool NeedsAttention { get; set; }
	}

	public class UserXp
	{
		public string PaymentProcessorCustomerID { get; set; }
	}

	public class BillingAddressXp
	{

	}

	public class LineItemXp
	{

	}

	public class ProductXp
	{
		public string TaxCode { get; set; }
	}

	public class VariantXp
	{

	}

	public class ShippingAddressXp
	{

	}

	public class SupplierAddressXp
	{

	}
 
	public class ShipEstimateResponseXp
	{

	}

	public class ShipEstimateXp
	{
		public ShippingPackage ShipPackage { get; set; }
	}

	public class ShipMethodXp
	{
		public string Carrier { get; set; }
	}

	public class OrderCalculateResponseXp
	{
		public OrderTaxCalculation TaxDetails { get; set; }
	}

	public class OrderSubmitResponseXp
	{
		/// <summary>
		/// Results of the post-submit processes
		/// </summary>
		public List<PostSubmitProcessResult> ProcessResults { get; set; }
	}

	public class OrderSubmitForApprovalResponseXp
	{

	}

	public class OrderApprovedResponseXp
	{

	}

	public class PaymentXp
	{
		public PCISafeCardDetails SafeCardDetails { get; set; }
	}

	public class PaymentTransactionXp
	{
		public CCTransactionResult TransactionDetails { get; set; }
	}
}
