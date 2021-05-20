namespace Catalyst.Common
{
	public class AppSettings
	{
		public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
		public ServiceBusSettings ServiceBusSettings { get; set; } = new ServiceBusSettings();
        public EnvironmentSettings EnvironmentSettings { get; set; } = new EnvironmentSettings();
		public RedisSettings RedisSettings { get; set; } = new RedisSettings(); // only need this if you use Redis
	}

	public class ServiceBusSettings
	{
		public string ConnectionString { get; set; }
		public string OrderProcessingQueueName { get; set; }
	}

	public class RedisSettings
	{
		public string ConnectionString { get; set; }
		public int DatabaseID { get; set; }
	}

	public class EnvironmentSettings
	{
		public string Env { get; set; } // typically "test", "qa", "stage", "prod", ect
		public string BuildNumber { get; set; } // Used in the build steps azure-pipeline.yaml
	}

	public class OrderCloudSettings
	{
		public string ApiUrl { get; set; } // 'https://api.ordercloud.io' or 'https://stagingapi.ordercloud.io' or 'https://sandboxapi.ordercloud.io'`
		public string MiddlewareClientID { get; set; } // Find this in the Ordercloud portal Api Client resource
		public string MiddlewareClientSecret { get; set; } // Find this in the Ordercloud portal Api Client resource
		public string WebhookHashKey { get; }  // Should match the HashKey configured on your webhook in the Ordercloud portal.
	}
}
