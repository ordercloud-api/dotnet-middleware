using System;
using System.Collections.Generic;
using System.Text;

namespace Catalyst.WebJobs
{
    public interface IAppSettings
    {
        OCSettings OrderCloudSettings { get; set; }
    }
    public class AppSettings : IAppSettings
    {
        public OCSettings OrderCloudSettings { get; set; } = new OCSettings();
    }
    public class OCSettings
    {
        public string ApiUrl { get; set; } // 'https://api.ordercloud.io' or 'https://stagingapi.ordercloud.io' or 'https://sandboxapi.ordercloud.io'`
        public string OrderCloudClientID { get; set; } // Find this in the Ordercloud portal Api Client resource
        public string OrderCloudClientSecret { get; set; } // Find this in the Ordercloud portal Api Client resource
        public string WebhookHashKey { get; }  // Should match the HashKey configured on your webhook in the Ordercloud portal.
    }
}
