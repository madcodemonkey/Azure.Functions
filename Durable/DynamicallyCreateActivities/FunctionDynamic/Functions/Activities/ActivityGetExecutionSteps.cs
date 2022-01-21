using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using FunctionDynamic.Models;

namespace FunctionDynamic
{
    public class ActivityGetExecutionSteps
    {

        [FunctionName(nameof(A_GetExecutionSteps))]
        public List<ExecutionStep> A_GetExecutionSteps([ActivityTrigger] int jobId, ILogger log)
        {
            // Note: We could just as easily get these setting from a database!

            var result = new List<ExecutionStep>();

            if (jobId == 1)
            {
                result.Add(new ExecutionStep(nameof(ActivityHello.A_Hello), nameof(ActivityHello.A_Hello)));
                result.Add(new ExecutionStep(nameof(ActivityRabbit.A_RabbitGood), nameof(ActivityRabbit.A_RabbitRollback)));
                result.Add(new ExecutionStep(nameof(ActivityPapa.A_Papa), nameof(ActivityPapa.A_Papa)));
            }
            else if (jobId == 2)
            {
                result.Add(new ExecutionStep(nameof(ActivityHello.A_Hello), nameof(ActivityHello.A_Hello)));
                result.Add(new ExecutionStep(nameof(ActivityRabbit.A_RabbitPartial), nameof(ActivityRabbit.A_RabbitRollback)));
                result.Add(new ExecutionStep(nameof(ActivityPapa.A_Papa), nameof(ActivityPapa.A_Papa)));
            }
            else if (jobId == 3)
            {
                result.Add(new ExecutionStep(nameof(ActivityHello.A_Hello), nameof(ActivityHello.A_Hello)));
                result.Add(new ExecutionStep(nameof(ActivityRabbit.A_RabbitFailure), nameof(ActivityRabbit.A_RabbitRollback)));
                result.Add(new ExecutionStep(nameof(ActivityPapa.A_Papa), nameof(ActivityPapa.A_Papa)));
            }
            else if (jobId == 4)
            {
                result.Add(new ExecutionStep(nameof(ActivityHello.A_Hello), nameof(ActivityHello.A_Hello)));

                // Rollback...either with or without retry
                result.Add(new ExecutionStep(nameof(ActivityRabbit.A_RabbitException), nameof(ActivityRabbit.A_RabbitRollback)));
                // result.Add(new ExecutionStep(nameof(ActivityRabbit.A_RabbitException), nameof(ActivityRabbit.A_RabbitRollback), 1, 2));  // Retry after 1 minute 2 times!

                result.Add(new ExecutionStep(nameof(ActivityPapa.A_Papa), nameof(ActivityPapa.A_Papa)));
            }
            return result;
        }

    }
}