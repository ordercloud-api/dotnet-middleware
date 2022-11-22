using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Models.Xp
{
	public partial class MessageSenderXp
	{
		public List<MessageTypeConfig> MessageTypeConfig { get; set; } = new List<MessageTypeConfig>();
	}

	public class MessageTypeConfig
	{
		public string MessageType { get; set; }
		public string FromEmail { get; set; }
		public string Subject { get; set; }
		public string TemplateName { get; set; }
	}
}
