using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionDynamic.Models;

namespace FunctionDynamic
{
    public class OrchestratorMain
    {
        [FunctionName(nameof(MainOrchestrator))]
        public async Task MainOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            // WARNING!!! You are within the context of an orchestrator function.  
            // WARNING!!! All code used here should be deterministic (https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints#using-deterministic-apis)
            // WARNING!!! For example, you cannot make calls to the database or other apis!!  All your work should be done via Activity functions!!!
            // WARNING!!! Keep in mind that everything returned from an activity function is serialized to blob storage so keep your return results small!!
            logger = context.CreateReplaySafeLogger(logger);
            int jobId = context.GetInput<int>();

            var workItems = await context.CallActivityAsync<List<ExecutionStep>>(nameof(ActivityGetExecutionSteps.A_GetExecutionSteps), jobId);

            ActivityData activityData = new ActivityData { JobId = jobId };

            foreach (ExecutionStep item in workItems)
            {
                await context.CallActivityAsync(nameof(ActivityHeartbeat.A_DoJobHeartbeat), activityData);

                activityData = await ExecuteStepAsync(context, item, activityData, logger);

                // The execution step has signaled that it failed so stop!
                if (activityData.Status == ExecutionStatus.Failure)
                {
                    break;
                }
            }


            if (activityData.Status == ExecutionStatus.Failure)
            {
                logger.LogError($"Log a failure for this reason: {activityData.LogText}");
            }
            else if (activityData.Status == ExecutionStatus.Partial)
            {
                logger.LogWarning($"Job id {jobId}  Partial Failure for this reason: {activityData.LogText}");
            }
            else
            {
                logger.LogInformation($"Job id {jobId} Done Success!");
            }
        }

        private async Task<ActivityData> ExecuteStepAsync(IDurableOrchestrationContext context, ExecutionStep executionStep, ActivityData data, ILogger logger)
        {
            // WARNING!!! You are within the context of an orchestrator function.  
            // WARNING!!! All code used here should be deterministic (https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints#using-deterministic-apis)
            // WARNING!!! For example, you cannot make calls to the database or other apis!!  All your work should be done via Activity functions!!!
            // WARNING!!! Keep in mind that everything returned from an activity function is serialized to blob storage so keep your return results small!!

            if (executionStep == null)
            {
                throw new ArgumentNullException(nameof(executionStep), "Execution step was not specified!");
            }

            try
            {
                if (executionStep.RetryFirstIntervalInMinutes > 0 && executionStep.RetryMaxNumberOfAttempts > 0)
                {
                    TimeSpan firstRetryInterval = new TimeSpan(0, 0, executionStep.RetryFirstIntervalInMinutes.Value, 0);
                    RetryOptions retryOptions = new RetryOptions(firstRetryInterval, executionStep.RetryMaxNumberOfAttempts.Value);
                    data = await context.CallActivityWithRetryAsync<ActivityData>(executionStep.PrimaryActivityFunctionName, retryOptions, data);
                }
                else
                {
                    data = await context.CallActivityAsync<ActivityData>(executionStep.PrimaryActivityFunctionName, data);
                }
            }
            catch (Exception ex)
            {
                var message = $"An error occurred while trying to execute the {executionStep.PrimaryActivityFunctionName} activity function step.";
                logger.LogError(ex, message);
                data.TryMarkAsFailure(message);

                data = await RollbackStepAsync(context, executionStep, data, logger);
            }

            return data;

        }

        private async Task<ActivityData> RollbackStepAsync(IDurableOrchestrationContext context, ExecutionStep executionStep, ActivityData data, ILogger logger)
        {
            // WARNING!!! You are within the context of an orchestrator function.  
            // WARNING!!! All code used here should be deterministic (https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-code-constraints#using-deterministic-apis)
            // WARNING!!! For example, you cannot make calls to the database or other apis!!  All your work should be done via Activity functions!!!
            // WARNING!!! Keep in mind that everything returned from an activity function is serialized to blob storage so keep your return results small!!

            if (string.IsNullOrWhiteSpace(executionStep.RollbackActivityFunctionName) == false)
            {
                logger.LogError($"Attempting to rollback the primary activity named {executionStep.PrimaryActivityFunctionName} by calling the rollback activity function called {executionStep.RollbackActivityFunctionName}"!);
                data = await context.CallActivityAsync<ActivityData>(executionStep.RollbackActivityFunctionName, data);
            }
            else
            {
                logger.LogInformation($"No rollback activity was specified for the primary activity function named {executionStep.PrimaryActivityFunctionName}!");
            }

            return data;
        }
    }
}