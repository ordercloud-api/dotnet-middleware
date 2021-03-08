namespace Catalyst.Api
{
	public class AppSettings
	{
		public OrderCloudSettings OrderCloudSettings { get; set; } = new OrderCloudSettings();
		public EnvironmentSettings EnvironmentSettings { get; set; } = new EnvironmentSettings();
		public RedisSettings RedisSettings { get; set; } = new RedisSettings(); // only need this if you use Redis
	}

	public class RedisSettings
	{
		public string ConnectionString { get; set; }
		public int DatabaseID { get; set; }
	}

	public class EnvironmentSettings
	{
		public string Env { get; set; } // typically "test", "qa", "stage", "prod", ect
		public string BuildNumber { get; set; }
	}

	public class OrderCloudSettings
	{
		public string ApiUrl { get; set; } // 'https://api.ordercloud.io/v1' or 'https://stagingapi.ordercloud.io/v1' or 'https://sandboxapi.ordercloud.io/v1'`
		public string MiddlewareClientID { get; set; } // Find this in the Ordercloud portal Api Client resource
		public string MiddlewareClientSecret { get; set; } // Find this in the Ordercloud portal Api Client resource
		public string WebhookHashKey { get; }  // Should match the HashKey configured on your webhook in the Ordercloud portal.
	}
}
