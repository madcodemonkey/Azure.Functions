using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using TimedJobPattern.Modals;

namespace TimedJobPattern;

public class OrchestratorMainFunction
{
    [Function(nameof(O_MainOrchestrator))]
    public static async Task<string> O_MainOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        // WARNING!!! You are within the context of an orchestrator function.  
        // WARNING!!! All code used here should be deterministic (https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints#using-deterministic-apis)
        // WARNING!!! For example, you cannot make calls to the database or other apis!!  All your work should be done via Activity functions!!!
        // WARNING!!! Keep in mind that everything returned from an activity function is serialized to blob storage so keep your return results small!!

        ILogger logger = context.CreateReplaySafeLogger(nameof(OrchestratorMainFunction));
        var workerId = context.GetInput<Guid>();
        string result;

        var workerActivityInformation = new WorkerActivityInformation { WorkerId = workerId, InstanceId = context.InstanceId };

        try
        {
            await context.CallActivityAsync(nameof(ActivityHeartBeatFunction.A_DoHeartBeatAsync), workerActivityInformation);

            var wasSuccessful = await context.CallActivityAsync<bool>(nameof(ActivityWorkFunction.A_DoWorkAsync), workerActivityInformation);
            result = wasSuccessful ? "Work was successful!" : "Work was a failure!";

            await context.CallActivityAsync(nameof(ActivityComputeNextRunFunction.A_ComputeNextRunAsync), workerActivityInformation);
        }
        catch (Exception ex)
        {
            result = "Work generated an unhandled exception!";
            logger.LogError(ex, $"An unhandled exception was caught in orchestrator for worker id {workerId} for instance id {context.InstanceId}!");
        }

        logger.LogInformation($"Finished with: {result}");
        
        return result;
    }
}