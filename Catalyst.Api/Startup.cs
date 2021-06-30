using OrderCloud.Catalyst;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderCloud.SDK;
using Microsoft.OpenApi.Models;
using Catalyst.Common;
using Catalyst.Common.Commands;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Catalyst.Common.Services;

namespace Catalyst.Api
{
	public class Startup
	{
		private readonly AppSettings _settings;

		public Startup(AppSettings settings) {
			_settings = settings;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public virtual void ConfigureServices(IServiceCollection services) {
			services
				.ConfigureServices()
				.AddOrderCloudUserAuth()
				.AddOrderCloudWebhookAuth(opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
				.AddSingleton<ISimpleCache, LazyCacheService>() // Replace LazyCacheService with RedisService if you have multiple server instances.
				.AddSingleton<IOrderCloudClient>(new OrderCloudClient(new OrderCloudClientConfig() {
					ApiUrl = _settings.OrderCloudSettings.ApiUrl,
					AuthUrl = _settings.OrderCloudSettings.ApiUrl,
					ClientId = _settings.OrderCloudSettings.MiddlewareClientID,
					ClientSecret = _settings.OrderCloudSettings.MiddlewareClientSecret,
				}))
				.Configure<KestrelServerOptions>(options =>
				{
					options.AllowSynchronousIO = true;
				})
				.Configure<IISServerOptions>(options =>
				{
					options.AllowSynchronousIO = true; // catalyst bug https://four51.atlassian.net/browse/HDS-190
				})
				.AddSingleton<IAzureServiceBus, AzureServiceBus>()
				.AddSingleton<ICheckoutIntegrationCommand, CheckoutIntegrationCommand>()
				.AddSwaggerGen(c =>
				 {
					 c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalyst Test API", Version = "v1" });
				 });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			CatalystApplicationBuilder.DefaultCatalystAppBuilder(app, env)
				.UseSwagger()
				.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"Catalyst Test API v1");
					c.RoutePrefix = string.Empty;
				});
		}
	}
}
