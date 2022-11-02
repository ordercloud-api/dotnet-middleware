using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models
{
	public partial class OrderXp
	{
		/// <summary>
		/// True if one of the post-submit processes failed
		/// </summary>
		public bool NeedsAttention { get; set; }
	}
}
