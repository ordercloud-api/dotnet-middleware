# Adding Settings Locally
1. Open the Headstart.sln in Visual Studio
2. Right click on the Headstart.Functions project and click on "Properties"
3. Go to the "Debug" panel
4. Create a new profile and call it "Test"
5. Set Launch to Project
6. Add a new Environment Variable called `APP_CONFIG_CONNECTION` and set the value to the connection string from your [azure app configuration](https://docs.microsoft.com/en-us/azure/azure-app-configuration/overview)
7. Save your profiles
8. Repeat steps 5-7 for each of your environments.
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