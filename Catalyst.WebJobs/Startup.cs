using System;
using System.Collections.Generic;
using System.Text;
using Catalyst.WebJobs;
using OrderCloud.SDK;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Catalyst.WebJobs
{
    public class Startup : FunctionsStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var _settings = BuildAppSettings();
            var orderCloudClient = new OrderCloudClient(new OrderCloudClientConfig
            {
                ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                ClientId = _settings.OrderCloudSettings.OrderCloudClientID,
                ClientSecret = _settings.OrderCloudSettings.OrderCloudClientSecret,
                Roles = new[] { ApiRole.FullAccess }
            });
            builder.Services.AddSingleton<IAppSettings>((s) => _settings);
            builder.Services.AddSingleton<IOrderCloudClient>(provider => orderCloudClient);
        }

        // This method is used to load setting to be used to initiate the OrderCloudClient.
        // For local testing create a appSettings.Development.json file.  Example:
        /*
            {
                "OrderCloudSettings": {
                    "ApiUrl": "https://sandboxapi.ordercloud.io",
                    "OrderCloudClientID": "YOURCLIENTID",
                    "OrderCloudClientSecret": "YOURCLIENTSECRET"
                }
            }
         */
        // For local testing set an ASPNETCORE_ENVIRONMENT Environment Variable in the Debug properties for the project.
        private static AppSettings BuildAppSettings()
        {
            var builder = new ConfigurationBuilder();
            
            builder.AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appSettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true) //for local dev only. this file should be excluded from the repo to avoid putting sensitive keys in the repo
                .AddEnvironmentVariables();

            var build = builder.Build();
            return build.Get<AppSettings>();
        }
    }
}
