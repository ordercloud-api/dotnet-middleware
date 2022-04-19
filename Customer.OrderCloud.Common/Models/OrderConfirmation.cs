using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
    public class OrderConfirmation
    {
        public OrderWorksheetWithXp OrderWorksheet { get; set; }
        public List<PaymentWithXp> Payments { get; set; }
    }
}
