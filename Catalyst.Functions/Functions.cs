using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using OrderCloud.SDK;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using MessageSender = Microsoft.Azure.ServiceBus.Core.MessageSender;
using Catalyst.Common.ProductUpload.Commands;
using Catalyst.Functions.Jobs.ForwardOrdersToThirdParty;

namespace Catalyst.Functions
{
    public class Functions
    {
        private readonly IOrderCloudClient _oc;
        private readonly IProductCommand _product;
        private readonly ForwardOrderJob _forwardJob;

        public Functions(IOrderCloudClient oc, IProductCommand product, ForwardOrderJob forwardJob)
        {
            _oc = oc;
            _product = product;
            _forwardJob = forwardJob;
        }

        // A simple example of a job that prints the authenticated OrderCloud username every 5 mins.
        [FunctionName("PrintUsername")]
        public async Task PrintToken([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger logger)
		{
            var user = await _oc.Me.GetAsync();
            logger.LogInformation($"Username: " + user.Username);
        }
        
        // Fire at 2:30am daily
        [FunctionName("BulkUploadProducts")]
        public async Task BulkUploadProducts([TimerTrigger("0 30 2 * * *")] TimerInfo myTimer, ILogger logger) => 
            await _product.ProcessSampleProducts(@"/ProductUpload/example_products.json", logger);

        [FunctionName("ForwardOrder")]
        public async Task ForwardOrder(
       [ServiceBusTrigger(
            queueName: "%ServiceBusSettings:OrderProcessingQueueName%",  // queueName can be stored in your app Configuration as it is here, or hard coded.
            Connection = "ServiceBusSettings:ConnectionString")]
        Message message,
       MessageReceiver messageReceiver,
       [ServiceBus(
            queueOrTopicName: "%ServiceBusSettings:OrderProcessingQueueName%",
            Connection = "ServiceBusSettings:ConnectionString" )]
        MessageSender messageSender,
       ILogger logger) => await _forwardJob.Run(logger, message, messageReceiver, messageSender);
    }
}
