using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using TimedJobPattern.Modals;
using TimedJobPattern.Services;

namespace TimedJobPattern.Functions
{
    public class TimerFunction
    {
        private readonly IWorkerService _workerService;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public TimerFunction(ILoggerFactory loggerFactory, IWorkerService workerService)
        {
            _workerService = workerService;
            _logger = loggerFactory.CreateLogger<TimerFunction>();
        }

        /// <summary>
        /// The timer that will check on orchestrator status and start new orchestrators.
        /// </summary>
        /// <param name="myTimer">Timer info</param>
        /// <param name="client">The client that allows us to start orchestrators</param>
        /// <param name="cancellationToken">A cancellation token</param>
        [Function("TimerFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer, [DurableClient] DurableTaskClient client,
            CancellationToken cancellationToken)
        {
            await FindAndTerminateDeadWorkersAsync(client, cancellationToken);

            await FindAndStartPendingWorkersAsync(client);

            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        /// <summary>
        /// Looks for workers that need to do work.
        /// </summary>
        /// <param name="client">The client that allows us to start orchestrators</param>
        private async Task FindAndStartPendingWorkersAsync(DurableTaskClient client)
        {
            _logger.LogInformation($"Starting to look for pending workers at: {DateTime.Now}");

            var workPending = await _workerService.FindWorkersThatNeedToRunAsync();
            foreach (WorkerConfiguration pendingWorker in workPending)
            {
                string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                    nameof(OrchestratorMainFunction.O_MainOrchestrator), pendingWorker.Id);

                _logger.LogInformation($"Starting worker with id {pendingWorker.Id} with a new orchestrator with instance id {instanceId}!");

                pendingWorker.InstanceId = instanceId;
                pendingWorker.LastHeartbeatTime = DateTime.UtcNow;
                await _workerService.SaveWorkerConfigurationAsync(pendingWorker);
            }

            _logger.LogInformation($"Finished looking for pending workers at: {DateTime.Now} and we started {workPending.Count} new workers!");
        }

        /// <summary>
        /// Looks for dead workers and terminates them.
        /// </summary>
        /// <param name="client">The client that allows us to start orchestrators</param>
        /// <param name="cancellationToken">A cancellation token</param>
        private async Task FindAndTerminateDeadWorkersAsync(DurableTaskClient client, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting to look for dead workers at: {DateTime.Now}");

            var deadWorkers = await _workerService.FindDeadWorkersAsync();

            if (deadWorkers.Count > 0)
            {
                _logger.LogError($"We found {deadWorkers.Count} dead workers.  We will attempt to terminate them!");
            }

            foreach (WorkerConfiguration deadWorker in deadWorkers)
            {
                _logger.LogWarning(
                    $"Attempting to terminate the worker with an id of {deadWorker.Id} and instance id of {deadWorker.InstanceId}!");

                if (string.IsNullOrWhiteSpace(deadWorker.InstanceId) == false)
                {
                    OrchestrationMetadata? orchestrationMetadata = await client.GetInstancesAsync(deadWorker.InstanceId, cancellationToken);
                    if (orchestrationMetadata == null)
                    {
                        _logger.LogWarning($"No metadata was found for  instance id {deadWorker.InstanceId}, so we will not " +
                                           $"attempt to terminate the worker with an id of {deadWorker.Id}!  " +
                                           $"We will clear the instance id in the db to avoid looking it up again!");
                    }
                    else if (orchestrationMetadata.RuntimeStatus == OrchestrationRuntimeStatus.Pending ||
                             orchestrationMetadata.RuntimeStatus == OrchestrationRuntimeStatus.Suspended ||
                             orchestrationMetadata.RuntimeStatus == OrchestrationRuntimeStatus.Running)
                    {
                        try
                        {
                            await client.TerminateInstanceAsync(deadWorker.InstanceId, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Attempt to terminate instance {deadWorker.InstanceId} has failed!  " +
                                                 $"We will clear the instance id in the db to avoid another failure!");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"The status of instance id is {orchestrationMetadata.RuntimeStatus}, so we will not " +
                                           $"attempt to terminate the worker with an id of {deadWorker.Id}!  " +
                                           $"We will clear the instance id in the db to avoid looking it up again!");
                    }
                }

                deadWorker.InstanceId = null;
                await _workerService.SaveWorkerConfigurationAsync(deadWorker);
            }

            _logger.LogInformation($"Finished looking for dead workers at: {DateTime.Now}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; } = default!;

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}