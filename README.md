# dotnet-middleware
Starter .NET microservice project for [Ordercloud](https://ordercloud.io/) extensions using the [Dotnet Catalyst](https://github.com/ordercloud-api/ordercloud-dotnet-catalyst) library. 

### Get started with Docker

- Clone the repo and build locally with `docker build -t MyOrderCloudMiddleware .`
- After building, run locally with `docker run -it -p 3000:80 MyOrderCloudMiddleware` then open http://localhost:3000/ in the browser and you should see api route documentation.

- Can also run a static example api from dockerhub with `docker run -it -p 3000:80 oliverheywood/ordercloud-dotnet-middleware`.

### Azure

See a working version of this api at https://headstartdemo-middleware-checkout-integrations.azurewebsites.net/index.html.
This is built and published to Azure on pushes to dev using a github action. 

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

### [Integration Events](./Customer.OrderCloud.Api/Controllers/IntegrationEventController.cs) 
 
Learn how to plug in your preferred shipping, tax, and payment processing partners to Ordercloud. Among other integrations.

### [Import Product Data](./Customer.OrderCloud.Common/Jobs/ProductUpload)

Learn how to import a product catalog to Ordercloud and keep it in sync with updates.

### [Forward Submitted Orders](./Customer.OrderCloud.Common/Jobs/ForwardOrder) 

Learn how to forward orders captured in ordercloud to a downstream system that handles fulfillment.

### [Proxy an API endpoint to extend functionality](./Customer.OrderCloud.Api/Controllers/ProxyListOrdersController.cs)

Learn how to wrap an Ordercloud endpoint in your own hosted API to perform logic in a secure environment. 

### [Custom Message Senders](./Customer.OrderCloud.Api/Controllers/MessageSenderController.cs)

Learn how to notify users of important events with custom emails.

