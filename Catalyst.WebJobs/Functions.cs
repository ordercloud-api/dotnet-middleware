using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using OrderCloud.SDK;

namespace Catalyst.WebJobs
{
    public class Functions
    {
        private readonly IOrderCloudClient _oc;

        public Functions(IOrderCloudClient oc)
        {
            _oc = oc;
        }

        public async Task SampleFunction(ILogger log)
        {
            // Get a token that can be used to make calls to the OrderCloud API using the OrderCloud SDK.
            var token = await GetTokenAsync();
            log.LogInformation($"Token: " + token);
        }

        // Fire every 5 minutes
        [FunctionName("SampleOrderCloudFunction")]
        public async Task SampleOrderCloudFunction([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger logger) => await SampleFunction(logger);

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
