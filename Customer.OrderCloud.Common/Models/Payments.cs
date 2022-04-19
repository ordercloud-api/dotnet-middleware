using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
    public class PaymentWithXp : Payment<PaymentXp, PaymentTransaction> { }

    public class PaymentXp
    {
        public PCISafeCardDetails SafeCardDetails { get; set; }
    }

    public class CreditCardPayment
    {
        public OrderDirection OrderDirection { get; set; }
        public string OrderID { get; set; }
        // in the currency on the order object
        public decimal Amount { get; set; }
        public string SavedCardID { get; set; }
        // If set, this will take priority over SavedCardID
        public PCISafeCardDetails CardDetails { get; set; }
        public bool SaveCardDetailsForFutureUse { get; set; }
	}
}
