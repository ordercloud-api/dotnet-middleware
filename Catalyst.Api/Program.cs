using Catalyst.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OrderCloud.Catalyst;
using System;

namespace Catalyst.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Links to an Azure App Configuration resource that holds the app settings.
			// For local development, set this in your visual studio Env Variables.
			var connectionString = Environment.GetEnvironmentVariable("APP_CONFIG_CONNECTION");

			CatalystWebHostBuilder
				.CreateWebHostBuilder<Startup, AppSettings>(args, connectionString)
				// .CreateWebHostBuilder<Startup, AppSettings>(args)
				//  If not using Azure App Configuration, use this line instead of above.
				.Build()
				.Run();
		}
	}
}
