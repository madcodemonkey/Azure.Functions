# Powershell Azure Service Bus example.
This has two projects.  One is the Azure Function and the other helps send a message to the service bus.

# You'll need a service bus namespace
- Create one in the Azure Portal
- Create a topic (I named mine davetopic) and then create a subscription underneath it (I named mine mySub)
- Get a connection for it that you will use in the Powershell project and console app.

# Opening the Azure Function Powershell project.
This was created with Visual Studio Code using these extensions:
- Azure Account 
- Azure Tools

# AzureFunctionPowerShell folder changes (Azure Function Powershell project)
- Modify the local.settings.json file and add a connection string to your service bus in the MyServiceBusConnectionKeyName key.
- Modify the service bus topic name in the function.json 
- Modify the service bus subscription name in the function.json 

# ConsoleSendServiceBusMessage folder changes 
- Open the Program.cs file
   - Add a connection string to your service bus in the myServiceBusConnectionString static variable at the top.
   - Modify the service bus topic name in the topicName static variable at the top.
   

# Running
Run the Powershell app by opening the run.ps1 file and hitting F5

Run the console application to send a message across the service bus 
