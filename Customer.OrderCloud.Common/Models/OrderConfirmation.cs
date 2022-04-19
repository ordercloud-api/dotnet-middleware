using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
    public class OrderConfirmation
    {
        public OrderWorksheet OrderWorksheet { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
