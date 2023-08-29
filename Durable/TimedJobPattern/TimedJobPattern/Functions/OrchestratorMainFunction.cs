using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace TimedJobPattern;

public class OrchestratorMainFunction
{
    [Function(nameof(MainOrchestrator))]
    public static async Task<List<string>> MainOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        // WARNING!!! You are within the context of an orchestrator function.  
        // WARNING!!! All code used here should be deterministic (https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints#using-deterministic-apis)
        // WARNING!!! For example, you cannot make calls to the database or other apis!!  All your work should be done via Activity functions!!!
        // WARNING!!! Keep in mind that everything returned from an activity function is serialized to blob storage so keep your return results small!!

        ILogger logger = context.CreateReplaySafeLogger(nameof(OrchestratorMainFunction));
        logger.LogInformation("Saying hello.");
        var outputs = new List<string>();

        // Replace name and input with values relevant for your Durable Functions Activity
        outputs.Add(await context.CallActivityAsync<string>(nameof(ActivitySayHelloFunction.SayHello), "Tokyo"));
        outputs.Add(await context.CallActivityAsync<string>(nameof(ActivitySayHelloFunction.SayHello), "Seattle"));
        outputs.Add(await context.CallActivityAsync<string>(nameof(ActivitySayHelloFunction.SayHello), "London"));

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

  
}