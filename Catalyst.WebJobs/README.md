# Getting Started
1. Open the Headstart.sln in Visual Studio
2. Add a "local.settings.json" file at the room of Headstart.Jobs with the following content:
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*",
    "CORSCredentials": false
  },
  "ConnectionStrings": {

  }
}
3. Right click on the Headstart.Jobs project and click on "Properties"
4. Go to the "Debug" panel
5. Create a new profile and call it "Test"
6. Set Launch to Project
7. Add a new Environment Variable called `APP_CONFIG_CONNECTION` and set the value to the connection string from your [azure app configuration](https://docs.microsoft.com/en-us/azure/azure-app-configuration/overview)
8. Save your profiles
9. Repeat steps 5-7 for each of your environments.
Now you should be able to select each debug profile to run the project locally

# Debugging locally
1. Run the Headstart.Jobs project locally selecting a profile created in #getting-started
2. [Open Postman](https://www.postman.com/)
3. POST to http://localhost:7071/admin/functions/{function_name} with a JSON body {"input": ""}
4. Click Send

# Accessing production logs
1. Navigate to your azure function
2. Select the correct environment via "Deployment Slots" in the left-hand menu
3. On the left hand menu click "Functions"
4. Select the function you're interested in
5. On the left hand menu click "Monitor"

# Timer Syntax
Azure functions uses the [NCrontab library](https://github.com/atifaziz/NCrontab#ncrontab-crontab-for-net) under the hood. You may see examples in azure using the six-part format which allows you to represent seconds. Generally that's too granular for us to care about so prefer to use the five-part format that is accurate to just minutes. Its more commonly used and you can plug it into tools like [this online crontab calculator](https://crontab.guru/)

Just remember the time represented in crontab is in the UTC timezone!