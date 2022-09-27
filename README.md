# dotnet-middleware
Starter .NET microservice project for [Ordercloud](https://ordercloud.io/) extension using the [Dotnet Catalyst](https://github.com/ordercloud-api/ordercloud-dotnet-catalyst) library. 

### Get Project Running With Docker

Clone the repo and build locally with `docker build -t MyOrderCloudMiddleware .`
After building, run locally with `docker run -it -p 3000:80 MyOrderCloudMiddleware` then open http://localhost:3000/api/env in the browser and you should see JSON data.

Can also run a static example api from dockerhub with `docker run -it -p 3000:80 oliverheywood/ordercloud-dotnet-middleware`.

### Why do I need middleware? 

OrderCloud is an API that handles commerce, so why do you need your own server-side project? Generally, the reason is that you want to securely extend the functionality of OrderCloud, either with custom logic or with functionality from another service. An ecommerce solution almost always integrates with external systems such as an ERP, a payment processor, an email automator, a shipper, a tax calculator, or others. For this reason, it is almost certain your OrderCloud solution will require middleware. 

OrderCloud middleware is usually built with variations on 3 primary patterns
- A webhook callback from OrderCloud to your API triggered by certain events
- A proxy route in your API that is called from the browser and then makes requests to OrderCloud with elevated permissions
- A scheduled job 

### [Start an API from Scratch](./Guides/SetupApi.md)

Learn how to stand up a hosted middleware API from scratch.

### [Configure Webhook Event Listeners](./Guides/Webhooks.md) 

Learn how to hook custom logic into any Ordercloud API request with Webhooks.

### [Checkout Integrations for Commerce](./Customer.OrderCloud.Common/Commands/CheckoutCommand.cs) 
 
Learn how to plug in your preferred shipping, tax, and payment processing partners to Ordercloud.

### [Import Product Data](./Customer.OrderCloud.Common/Jobs/ProductUpload)

Learn how to import a product catalog to Ordercloud and keep it in sync with updates.

### [Forward Submitted Orders](./Customer.OrderCloud.Common/Jobs/ForwardOrder) 

Learn how to forward orders captured in ordercloud to a downstream system that handles fulfillment.

### [Proxy an API endpoint to extend functionality](./Customer.OrderCloud.Api/Controllers/ProxyListOrdersController.cs)

Learn how to wrap an Ordercloud endpoint in your own hosted API to perform logic in a secure environment. 

