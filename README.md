# dotnet-catalyst-examples
Example middleware patterns for [Ordercloud](https://ordercloud.io/) using the [Dotnet Catalyst](https://github.com/ordercloud-api/ordercloud-dotnet-catalyst) library. 

#### Why do I need middleware? 

Developers building OrderCloud ecommerce apps often use a server-side project for a variety of features. For example, secure integrations with 3rd parties like payment processors, scheduled jobs that sync data like product imports, custom event triggered emails, and others. Its almost certain your ecommerce solution will require middleware.

### [Start an API from Scratch](./Guides/SetupApi.md)

Learn how to stand up a hosted middleware API from scratch.

### [Configure Webhook Event Listeners](./Guides/Webhooks.md) 

Learn how to hook custom logic into any Ordercloud API request with Webhooks.

### [Checkout Integrations for Commerce](./Catalyst.Api/Controllers/CheckoutIntegrationController.cs) 
 
Learn how to plug in your preferred shipping, tax, and payment processing partners to Ordercloud.

### [Import Product Data](./Catalyst.Common/Jobs/ProductUpload)

Learn how to import a product catalog to Ordercloud and keep it in sync with updates.

### [Forward Submitted Orders](./Catalyst.Common/Jobs/ForwardOrder) 

Learn how to forward orders captured in ordercloud to a downstream system that handles fulfillment.

### [Proxy an API endpoint to extend functionality](./Catalyst.Api/Controllers/ProxyListOrdersController.cs)

Learn how to wrap an Ordercloud endpoint in your own hosted API to perform logic in a secure environment. 

