using OrderCloud.Integrations.Messaging.MailChimp;
using OrderCloud.Integrations.Payment.Stripe;
using OrderCloud.Integrations.Shipping.EasyPost;
using OrderCloud.Integrations.Tax.Avalara;

namespace Catalyst.Common
{
	public class AppSettings
	{
		public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
		public ServiceBusSettings ServiceBusSettings { get; set; } = new ServiceBusSettings();
        public EnvironmentSettings EnvironmentSettings { get; set; } = new EnvironmentSettings();
		public EasyPostConfig EasyPostSettings { get; set; } = new EasyPostConfig();
		public AvalaraConfig AvalaraSettings { get; set; } = new AvalaraConfig();
		public StripeConfig StripeSettings { get; set; } = new StripeConfig();
		public MailChimpConfig MailChimpSettings { get; set; } = new MailChimpConfig();
	}

	public class ServiceBusSettings
	{
		public string ConnectionString { get; set; }
		public string OrderProcessingQueueName { get; set; }
	}

	public class EnvironmentSettings
	{
		public string Env { get; set; } // typically "test", "qa", "stage", "prod", ect
		public string BuildNumber { get; set; } // Used in the build steps azure-pipeline.yaml
	}

	public class OrderCloudSettings
	{
		public string StorefrontClientID { get; set; }
		public string ApiUrl { get; set; } // 'https://api.ordercloud.io' or 'https://stagingapi.ordercloud.io' or 'https://sandboxapi.ordercloud.io'`
		public string MiddlewareClientID { get; set; } // Find this in the Ordercloud portal Api Client resource
		public string MiddlewareClientSecret { get; set; } // Find this in the Ordercloud portal Api Client resource
		public string WebhookHashKey { get; }  // Should match the HashKey configured on your webhook in the Ordercloud portal.
	}
}
