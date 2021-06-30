using System;
using OrderCloud.SDK;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Catalyst.Common;
using OrderCloud.Catalyst;
using Catalyst.Common.ProductUpload.Commands;
using Catalyst.Functions;
using Catalyst.Functions.Jobs.ForwardOrdersToThirdParty;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Catalyst.Functions
{
    public class Startup : FunctionsStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION");
            var settings = builder.BuildSettingsFromAzureAppConfig<AppSettings>(connectionString);

            var orderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = settings.OrderCloudSettings.ApiUrl,
                AuthUrl = settings.OrderCloudSettings.ApiUrl,
                ClientId = settings.OrderCloudSettings.MiddlewareClientID,
                ClientSecret = settings.OrderCloudSettings.MiddlewareClientSecret,
                Roles = new[] { ApiRole.FullAccess }
            });
            builder.Services.AddSingleton(settings);
            builder.Services.AddSingleton<IOrderCloudClient>(orderCloudClient);
            builder.Services.AddSingleton<IProductCommand, ProductCommand>();
            builder.Services.AddSingleton<ForwardOrderJob>();

        }
    }           
}
