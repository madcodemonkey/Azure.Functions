# Azure Functions

## Projects List
- Common
   - AppInsightsEnhancedWithSerilogSink - A Serilog example that I was using for common logging (TODO: it needs work)
- Durable 
   - DynamicCreationOfActivities - Shows a durable function example where I dynamically run activities base on the way the HttpTrigger is called.
- Normal
   - DotNet6CoreIsolated - Shows serveral HttpTriggers in .NET 6 Core using the new Isolated process mode.
   - DotNet6CoreNotIsolated - Shows serveral HttpTriggers in .NET 6 Core using the older In Process mode.
- Powershell 
   - HttpTriggered - A PowerShell fired by a HttpTrigger running in an Azure Function (use VSCode to examine and run this code)
   - ServiceBusTriggered 
      - AzureFunctionPowerShell - A Powershell fired by a ServiceBus trigger in an Azure Function
      - ConsoleSendServiceBusMessage - A Console app to create messages for the PowerShell program (TODO: Delete this and use my Console App that I created under Azure.ServiceBus)
