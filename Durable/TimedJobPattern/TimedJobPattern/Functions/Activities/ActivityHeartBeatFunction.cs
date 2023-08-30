using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TimedJobPattern.Modals;
using TimedJobPattern.Services;

namespace TimedJobPattern;

public class ActivityHeartBeatFunction
{
    private readonly IWorkerService _workerService;

    /// <summary>
    /// Constructor
    /// </summary>
    public ActivityHeartBeatFunction(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    [Function(nameof(A_DoHeartBeatAsync))]
    public async Task A_DoHeartBeatAsync([ActivityTrigger] WorkerActivityInformation info, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(A_DoHeartBeatAsync)); 
        logger.LogInformation($"Doing heartbeat for worker id {info.WorkerId} with instance {info.InstanceId}!");
        var workerConfig = await _workerService.GetWorkerConfigurationByIdAsync(info.WorkerId);
        if (workerConfig != null)
        {
            workerConfig.InstanceId = info.InstanceId;
            workerConfig.LastHeartbeatTime = DateTime.UtcNow;
            await _workerService.SaveWorkerConfigurationAsync(workerConfig);
            logger.LogInformation($"Worker id {info.WorkerId} heartbeat info saved!");
        }
        else
        {
            logger.LogError($"Heartbeat update failed. Unable to find a worker with an id of {info.WorkerId}!");
        }
    }
}