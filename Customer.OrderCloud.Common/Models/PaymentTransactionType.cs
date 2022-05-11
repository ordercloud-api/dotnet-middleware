using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
	public enum PaymentTransactionType
	{
		Authorization,
		Void,
		Capture,
		Refund
	}
}
