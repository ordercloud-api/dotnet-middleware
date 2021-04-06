using System;
using Catalyst.WebJobs;
using OrderCloud.SDK;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Catalyst.Common;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Catalyst.WebJobs
{
    public class Startup : FunctionsStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION");
            var _settings = builder.BuildSettingsFromAzureAppConfig<AppSettings>(connectionString);

            var orderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.OrderCloudSettings.MiddlewareClientID,
                ClientSecret = _settings.OrderCloudSettings.MiddlewareClientSecret,
                Roles = new[] { ApiRole.FullAccess }
            });
            builder.Services.AddSingleton(_settings);
            builder.Services.AddSingleton<IOrderCloudClient>(orderCloudClient);
        }
    }

    public static class FunctionHostBuilderExtensions
	{
        public static TSettings BuildSettingsFromAzureAppConfig<TSettings>(this IFunctionsHostBuilder host, string connectionString)
           where TSettings : class, new()
        {
            var settings = new TSettings();

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddAzureAppConfiguration(connectionString)
                .Build();

            config.Bind(settings);
            return settings;
        }
    }
              
}
