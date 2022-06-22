using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
    public class CreditCardPayment
    {
        public string OrderID { get; set; }
        public decimal Amount { get; set; } // in the currency on the order object
        public PCISafeCardDetails CardDetails { get; set; }
        public bool SaveCardForFutureUse { get; set; }
	}
}
