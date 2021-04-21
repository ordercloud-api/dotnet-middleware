# Configure Webhook Event Listeners 

### What is this guide?

This guide will show you how to create your own http endpoints that perform custom logic in response to requests initiated by the OrderCloud platfrom. These requests can be triggered by any action in the platform and are called webhooks. Before beginning, [read the guide "Using Webhooks"](https://ordercloud.io/knowledge-base/using-webhooks) in the knowledge base to familiarize yourself with concepts. This guide will focus on step-by-step instructions and code examples to get you to the point where you know webhooks are working. 

1. Get code and run API locally. 

First, clone this github project into your local file system and consider setting up your own version control repository. Then, run the api project locally. To do this, follow instructions in [Start an API from scratch](../SetupApi.md). Stop before the section "Publish API to Azure App Service". When you run the api locally and navigate to https://localhost:5001 you should see route documentation, including two example webhook routes.    

![Alt text](./webhook_route_docs.png "Route documentation for example webhooks")
