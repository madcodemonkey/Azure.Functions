using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TimedJobPattern.Modals;
using TimedJobPattern.Services;

namespace TimedJobPattern;

public class ActivityWorkFunction
{
    private readonly IWorkerService _workerService;

    /// <summary>
    /// Constructor
    /// </summary>
    public ActivityWorkFunction(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    [Function(nameof(A_DoWorkAsync))]
    public async Task<bool> A_DoWorkAsync([ActivityTrigger] WorkerActivityInformation info, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(A_DoWorkAsync));
        logger.LogInformation($"Doing work for worker id {info.WorkerId} with instance {info.InstanceId}!");

        var result = await _workerService.DoRandomWorkAsync(info);

        logger.LogInformation($"Finished work for worker id {info.WorkerId} with instance {info.InstanceId}!");

        return result;
        
    }
}