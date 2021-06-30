using Catalyst.Common;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;

namespace Catalyst.Api.Controllers
{
	[Route("api/env")]
	public class EnvController : CatalystController
	{
		private readonly AppSettings _settings;
		public EnvController(AppSettings settings)
		{
			_settings = settings;
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
	}
}
