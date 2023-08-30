using TimedJobPattern.Modals;

namespace TimedJobPattern.Services;

public interface IWorkerService
{
    Task<List<WorkerConfiguration>> GetAllWorkingItemsAsync();

    Task SaveWorkerConfigurationAsync(WorkerConfiguration newWorkerConfig);

    Task<WorkerConfiguration?> GetWorkerConfigurationByInstanceIdAsync(string instanceId);

    Task<WorkerConfiguration?> GetWorkerConfigurationByIdAsync(Guid id);

    Task<List<WorkerConfiguration>> GetAllRunningWorkersAsync();
    Task<List<WorkerConfiguration>> GetAllIdleWorkersAsync();

    Task<List<WorkerConfiguration>> FindDeadWorkersAsync();

    Task<List<WorkerConfiguration>> FindWorkersThatNeedToRunAsync();
    Task<bool> DoRandomWorkAsync(WorkerActivityInformation info);
}