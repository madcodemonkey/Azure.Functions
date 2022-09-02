# Azure Functions

## Projects List
- Common
   - AppInsightsEnhancedWithSerilogSink - A Serilog example that I was using for common logging (TODO: it needs work)
- Durable - durable function examples
- Event Hub - Azure functions with event hub trigger
- Normal
   - InProcessHttpTriggerExample - Shows serveral HttpTriggers in .NET 6 Core using the older In Process mode.
   - IsolatedHttpTriggerExample - Shows serveral HttpTriggers in .NET 6 Core using the new Isolated process mode.
- Powershell 
   - HttpTriggered - A PowerShell fired by a HttpTrigger running in an Azure Function (use VSCode to examine and run this code)
   - ServiceBusTriggered 
      - AzureFunctionPowerShell - A Powershell fired by a ServiceBus trigger in an Azure Function
      - ConsoleSendServiceBusMessage - A Console app to create messages for the PowerShell program (TODO: Delete this and use my Console App that I created under Azure.ServiceBus)
- Service Bus - Azure functions with service bus trigger (queue or topic)

## Branching scheme
- DotNet6: .NET Core 6.0 examples (with the current exception of AppInsightsEnhancedWithSerilogSink and PowerShell items)
- [Future] I will create a branch for each version of .NET 
