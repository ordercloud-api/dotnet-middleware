using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Collections.Generic;

namespace Customer.OrderCloud.Common.Models
{
	public partial class OrderXp 
	{
		/// <summary>
		/// True if one of the post-submit processes failed
		/// </summary>
		public bool NeedsAttention { get; set; }
	}

	public partial class UserXp
	{
		public string PaymentProcessorCustomerID { get; set; }
	}


	public partial class ProductXp
	{
		public string TaxCode { get; set; }
	}

	public partial class ShipEstimateXp
	{
		public ShippingPackage ShipPackage { get; set; }
	}

	public partial class ShipMethodXp
	{
		public string Carrier { get; set; }
	}

	public partial class OrderCalculateResponseXp
	{
		public OrderTaxCalculation TaxDetails { get; set; }
	}

	public partial class OrderSubmitResponseXp
	{
		/// <summary>
		/// Results of the post-submit processes
		/// </summary>
		public List<PostSubmitProcessResult> ProcessResults { get; set; }
	}

	public partial class PaymentXp
	{
		public PCISafeCardDetails SafeCardDetails { get; set; }
	}

	public partial class PaymentTransactionXp
	{
		public CCTransactionResult TransactionDetails { get; set; }
	}
}
