# dotnet-middleware
Starter .NET server-side integration project for [Ordercloud](https://ordercloud.io/) using the [Dotnet Catalyst](https://github.com/ordercloud-api/ordercloud-dotnet-catalyst) library. 

#### Why do I need middleware? 

One major reason is a secure environment to integrate OrderCloud with other software vendors. A full ecommerce solution often involves systems like an ERP (as a source of products and destination for orders), an email automator, a shipper, a tax calculator, a payment processor and others. In an OrderCloud solution these integrations are generally built with variations on 3 middleware patterns
- A webhook callback from OrderCloud 
- A proxy route called from the browser
- A scheduled job. 

Its almost certain your ecommerce solution will require middleware and use all these patterns.

### [Start an API from Scratch](./Guides/SetupApi.md)

Learn how to stand up a hosted middleware API from scratch.

### [Configure Webhook Event Listeners](./Guides/Webhooks.md) 

Learn how to hook custom logic into any Ordercloud API request with Webhooks.

### [Checkout Integrations for Commerce](./Customer.OrderCloud.Api/Controllers/CheckoutController.cs) 
 
Learn how to plug in your preferred shipping, tax, and payment processing partners to Ordercloud.

### [Import Product Data](./Customer.OrderCloud.Common/Jobs/ProductUpload)

Learn how to import a product catalog to Ordercloud and keep it in sync with updates.

### [Forward Submitted Orders](./Customer.OrderCloud.Common/Jobs/ForwardOrder) 

Learn how to forward orders captured in ordercloud to a downstream system that handles fulfillment.

### [Proxy an API endpoint to extend functionality](./Customer.OrderCloud.Api/Controllers/ProxyListOrdersController.cs)

Learn how to wrap an Ordercloud endpoint in your own hosted API to perform logic in a secure environment. 

