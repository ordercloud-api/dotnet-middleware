using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models
{
	public partial class OrderSubmitResponseXp
	{
		/// <summary>
		/// Results of the post-submit processes
		/// </summary>
		public List<PostSubmitProcessResult> ProcessResults { get; set; }
	}
}
