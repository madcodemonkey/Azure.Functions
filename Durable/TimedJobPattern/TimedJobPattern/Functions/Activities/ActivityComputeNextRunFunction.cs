using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TimedJobPattern.Modals;
using TimedJobPattern.Services;

namespace TimedJobPattern;

public class ActivityComputeNextRunFunction
{
    private readonly IWorkerService _workerService;
    private readonly IWorkerConfigurationService _workerConfigurationService;

    public ActivityComputeNextRunFunction(IWorkerService workerService, IWorkerConfigurationService workerConfigurationService)
    {
        _workerService = workerService;
        _workerConfigurationService = workerConfigurationService;
    }

    [Function(nameof(A_ComputeNextRunAsync))]
    public async Task A_ComputeNextRunAsync([ActivityTrigger] WorkerActivityInformation info, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(A_ComputeNextRunAsync));
        logger.LogInformation($"Computing the next run for worker id {info.WorkerId} with instance {info.InstanceId}!");
        var workerConfig = await _workerService.GetWorkerConfigurationByIdAsync(info.WorkerId);
        if (workerConfig != null)
        {
            workerConfig.InstanceId = null;
            workerConfig.LastRunTime = DateTime.UtcNow;
            workerConfig.LastHeartbeatTime = DateTime.UtcNow;
            workerConfig.NextRunTime = _workerConfigurationService.ComputeNextWorkerRun(workerConfig);

            await _workerService.SaveWorkerConfigurationAsync(workerConfig);
            logger.LogInformation($"Worker id {info.WorkerId} NextRunTime updated!");
        }
        else
        {
            logger.LogError($"Compute next run failed. Unable to find a worker with an id of {info.WorkerId}!");
        }
    }
}