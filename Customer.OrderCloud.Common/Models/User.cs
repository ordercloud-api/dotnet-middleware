using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace Customer.OrderCloud.Common.Models
{
    public class MeUserWithXp : MeUser<MeUserXp> { }

    public class MeUserXp
    {
        public string PaymentProcessorCustomerID { get; set; }
    }
}
