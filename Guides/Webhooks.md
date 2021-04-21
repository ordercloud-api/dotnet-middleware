# Configure Webhook Event Listeners 

### What is this guide?

This guide will show you how to create your own http endpoints that perform custom logic in response to requests initiated by the OrderCloud platfrom. These requests can be triggered by any action in the platform and are called webhooks. Before beginning, [read the guide "Using Webhooks"](https://ordercloud.io/knowledge-base/using-webhooks) in the knowledge base to familiarize yourself with concepts. This guide will focus on step-by-step instructions and code examples to get you to the point where you know webhooks are working. 

### Get code and run API locally. 

First, clone this github project into your local file system and consider setting up your own version control repository. Then, run the api project locally. To do this, follow instructions in [Start an API from scratch](../SetupApi.md). Stop before the section "Publish API to Azure App Service". When you run the api locally and navigate to https://localhost:5001 you should see route documentation, including two example webhook routes.    

![Alt text](./webhook_route_docs.png "Route documentation for example webhooks")

Lets look at the code in [WebhookController.cs](https://github.com/ordercloud-api/dotnet-catalyst-examples/blob/dev/Catalyst.Api/Controllers/WebhookController.cs) to make this happen.  

```c#
		[HttpPost("api/webhook/createaddress")]
		[OrderCloudWebhookAuth] 
		public PreWebhookResponse Task HandleOrderApprove([FromBody] WebhookPayloads.Addresses.Create payload)
		{
        ....
		}
```
`OrderCloudWebhookAuth` is a security feature that blocks requests that do not come from OrderCloud webhooks. The parameter type `WebhookPayloads.Addresses.Create` contains detailed info about a Create Address event in OrderCloud. The return type `PreWebhookResponse` lets us to block or allow the continuation of the create address logic in OrderCloud.

```c#
public virtual void ConfigureServices(IServiceCollection services) {
			services.AddOrderCloudWebhookAuth(opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
}
```
Set the app setting `WebhookHashKey` to a secret, arbitrary string. You will configure this string in OrderCloud to give webhooks access to your protected routes.  

### Using Ngrok for local development

When you are first setting up webhooks and later when you are developing the custom logic inside your routes, you'll want a better testing plan than publishing your changes to a hosted API and seeing if it works as expected. Seeing real OrderCloud webhooks requests on locally hosted API and stepping through the code with a debugger can be very helpful. The challenge is that you cannot simply provide OrderCloud with https://localhost:5001. The solution is a free tool called [Ngrok](https://ngrok.com/) that creates seccure tunnels from publically available urls to your middleware. 

Install Ngrok](https://ngrok.com/download), start up your local api and in a terminal run `ngrok http https://localhost:5001 -host-header="localhost:5001"`. This more complicated command allows https connections as opposed to http. In the terminal window, ngrok will supply you with a public forwarding host. In postman or however you prefer make a GET request to https://{ngrok-host}/api/env. You should see a json response from your locally running middleware and a log of the request in ngrok.  

![Alt text](./ngrok_forwarding.png "Running Ngrok")
