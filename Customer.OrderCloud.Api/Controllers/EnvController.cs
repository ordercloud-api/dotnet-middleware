using Catalyst.Common;
using Customer.OrderCloud.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;

namespace Customer.OrderCloud.Api.Controllers
{
	[Route("api/env")]
	public class EnvController : CatalystController
	{
		private readonly AppSettings _settings;
		private readonly OrderCloudWebhookRouteAnalyser _webhooks;
		public EnvController(AppSettings settings, OrderCloudWebhookRouteAnalyser webhooks)
		{
			_settings = settings;
			_webhooks = webhooks;
		}

		[HttpGet("")]
		public object GetEnvironment()
		{
			return new {
				_settings.EnvironmentSettings.BuildNumber,
				_settings.OrderCloudSettings.ApiUrl,
				_settings.OrderCloudSettings.MiddlewareClientID,
			};
		}

		[HttpGet("webhook")]
		public object GetWebhookConfig()
		{
			return _webhooks.AnalyzeProjectWebhookRoutes();
		}
	}
}
