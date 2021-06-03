using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using OrderCloud.SDK;
using Catalyst.WebJobs.ProductUpload.Commands;

namespace Catalyst.WebJobs
{
    public class Functions
    {
        private readonly IOrderCloudClient _oc;
        private readonly IProductCommand _product;

        public Functions(IOrderCloudClient oc, IProductCommand product)
        {
            _oc = oc;
            _product = product;
        }

        public async Task SampleFunction(ILogger log)
        {
            // Get a token that can be used to make calls to the OrderCloud API using the OrderCloud SDK.
            var token = await GetTokenAsync();
            log.LogInformation($"Token: " + token);
        }
        public async Task ProductUploadFunction(ILogger log)
        {
            // Get a token that can be used to make calls to the OrderCloud API using the OrderCloud SDK.
            var token = await GetTokenAsync();
            await _product.ProcessSampleProducts(@"/ProductUpload/example_products.json", token, log);
            log.LogInformation($"ProcessBrandwearProducts Complete.");
        }

        // Fire every 5 minutes
        [FunctionName("SampleOrderCloudFunction")]
        public async Task SampleOrderCloudFunction([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger logger) => await SampleFunction(logger);
        
        // Fire at 2:30am daily
        [FunctionName("SampleProductUploadFunction")]
        public async Task SampleProductUploadFunction([TimerTrigger("0 30 2 * * *")] TimerInfo myTimer, ILogger logger) => await ProductUploadFunction(logger);

        private async Task<string> GetTokenAsync()
        {
            var token = _oc.TokenResponse?.AccessToken;
            if (token == null || DateTime.UtcNow > _oc.TokenResponse.ExpiresUtc)
            {
                await _oc.AuthenticateAsync();
                token = _oc.TokenResponse?.AccessToken;
            }
            return token;
        }
    }
}
