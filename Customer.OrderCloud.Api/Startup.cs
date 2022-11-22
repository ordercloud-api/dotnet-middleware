using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderCloud.SDK;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Catalyst.Common;
using OrderCloud.Catalyst;
using Catalyst.Common.Services;
using Customer.OrderCloud.Common.Commands;
using OrderCloud.Integrations.Shipping.EasyPost;
using OrderCloud.Integrations.Tax.Avalara;
using OrderCloud.Integrations.Payment.Stripe;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Customer.OrderCloud.Api
{
	public class Startup
	{
		private readonly AppSettings _settings;

		public Startup(AppSettings settings) {
			_settings = settings;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public virtual void ConfigureServices(IServiceCollection services) {
			var stripeService = new StripeService(_settings.StripeSettings);
				
			services
				.AddControllers()
				.ConfigureApiBehaviorOptions(o =>
				{
					o.SuppressModelStateInvalidFilter = true;
				}).AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
				});
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
			services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));
			services.AddMvc(o =>
			{
				o.Filters.Add(new ValidateModelAttribute());
				o.EnableEndpointRouting = false;
			});
			services.AddCors(o => o.AddPolicy("integrationcors",
				builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
			services
				.AddEndpointsApiExplorer()
				.AddOrderCloudUserAuth(opts => opts.AddValidClientIDs(_settings.OrderCloudSettings.StorefrontClientID))
				.AddOrderCloudWebhookAuth(opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
				.AddSingleton<ISimpleCache, LazyCacheService>() // Replace LazyCacheService with RedisService if you have multiple server instances.
				.AddSingleton<IOrderCloudClient>(new OrderCloudClient(new OrderCloudClientConfig() {
					ApiUrl = _settings.OrderCloudSettings.ApiUrl,
					AuthUrl = _settings.OrderCloudSettings.ApiUrl,
					ClientId = _settings.OrderCloudSettings.MiddlewareClientID,
					ClientSecret = _settings.OrderCloudSettings.MiddlewareClientSecret,
				}))
				.AddSingleton<IAzureServiceBus, AzureServiceBus>()
				.AddSingleton<ICheckoutCommand, CheckoutCommand>()
				.AddSingleton<ICreditCardCommand, CreditCardCommand>()
				.AddSingleton<IShippingRatesCalculator>(new EasyPostService(_settings.EasyPostSettings))
				.AddSingleton<ITaxCalculator>(new AvalaraService(_settings.AvalaraSettings))
				.AddSingleton<ICreditCardProcessor>(stripeService)
				.AddSingleton<ICreditCardSaver>(stripeService)
				.AddSingleton<ISendEmailCommand, SendEmailCommand>()
				.AddSwaggerGen(c =>
				 {
					 c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderCloud Middleware API", Version = "v1" });
				 });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCatalystExceptionHandler();
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseCors("integrationcors");
			app.UseAuthorization();
			app.UseEndpoints(endpoints => endpoints.MapControllers());
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"OrderCloud Middleware API v1");
				c.RoutePrefix = string.Empty;
			});
		}
	}
}
