using TimedJobPattern.Modals;

namespace TimedJobPattern.Services;

public interface IWorkerConfigurationService
{
    /// <summary>
    /// Computes the next time the worker should run.
    /// </summary>
    /// <param name="workerConfig">The worker's configuration</param>
    DateTime ComputeNextWorkerRun(WorkerConfiguration workerConfig);
}