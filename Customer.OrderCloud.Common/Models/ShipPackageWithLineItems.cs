using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
	public class ShippingPackageWithLineItems
	{
		public ShippingPackage ShipPackage { get; set; }
		public IList<ShipEstimateItem> ShipEstimateItems { get; set; }
	}
}
