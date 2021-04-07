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
				//.CreateWebHostBuilder<Startup, AppSettings>(args, connectionString)
				// If use Azure App Configuration, uncomment the line above. If not, bind AppSettings as you choose.
				.CreateWebHostBuilder<Startup, AppSettings>(args)
				.Build()
				.Run();
		}
	}
}
