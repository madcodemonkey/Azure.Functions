using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TimedJobPattern.Modals;

namespace TimedJobPattern.Services;

public class WorkerService : IWorkerService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IMemoryCache _memoryCache;
    private const string WorkingItemsKey = "workingItems";
    private readonly ApplicationSettings _applicationSettings;

    /// <summary>
    /// Constructor
    /// </summary>
    public WorkerService(ILogger<WorkerService> logger, IMemoryCache memoryCache, IOptions<ApplicationSettings> appOptions)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        _applicationSettings = appOptions.Value;
    }

    public async Task<List<WorkerConfiguration>> GetAllWorkingItemsAsync()
    {
        List<WorkerConfiguration>? items = _memoryCache.Get<List<WorkerConfiguration>>(WorkingItemsKey);
        if (items == null)
        {
            items = new List<WorkerConfiguration>(2)
            {
                new() // Ready to start
                {
                    Id = Guid.NewGuid(),
                    AddIntervalToNextRunTime = true,
                    LastRunTime = DateTime.UtcNow.AddMinutes(-12),
                    NextRunTime = DateTime.UtcNow.AddMinutes(-1),
                    LastHeartbeatTime = null,
                    RunTimeIntervalInMinutes = 5
                },
                new() // Start in a minute
                {
                    Id = Guid.NewGuid(),
                    AddIntervalToNextRunTime = true,
                    LastRunTime = DateTime.UtcNow.AddMinutes(-6),
                    NextRunTime = DateTime.UtcNow.AddMinutes(1),
                    LastHeartbeatTime = null,
                    RunTimeIntervalInMinutes = 5
                }
            };

            _memoryCache.Set(WorkingItemsKey, items);
        }

        return items;
    }

    public async Task<List<WorkerConfiguration>> GetAllRunningWorkersAsync()
    {
        var result = new List<WorkerConfiguration>();

        List<WorkerConfiguration> items = await GetAllWorkingItemsAsync();
        foreach (WorkerConfiguration item in items)
        {
            if (!string.IsNullOrWhiteSpace(item.InstanceId))
                result.Add(item);
        }

        return result;
    }

    public async Task<List<WorkerConfiguration>> GetAllIdleWorkersAsync()
    {
        var result = new List<WorkerConfiguration>();

        List<WorkerConfiguration> items = await GetAllWorkingItemsAsync();
        foreach (WorkerConfiguration item in items)
        {
            if (string.IsNullOrWhiteSpace(item.InstanceId))
                result.Add(item);
        }

        return result;
    }

    public async Task<List<WorkerConfiguration>> FindDeadWorkersAsync()
    {
        var result = new List<WorkerConfiguration>();

        List<WorkerConfiguration> items = await GetAllRunningWorkersAsync();
        foreach (WorkerConfiguration item in items)
        {
            if (item.LastHeartbeatTime == null)
            {
                // Item is running (has an instance id), but has not recorded a heartbeat.
                result.Add(item);
            }
            else
            {
                var timeElapsed = DateTime.UtcNow - item.LastHeartbeatTime.Value;
                if (timeElapsed.TotalMinutes > _applicationSettings.MaximumIdleTimeInMinutes)
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }

    public async Task<List<WorkerConfiguration>> FindWorkersThatNeedToRunAsync()
    {
        var result = new List<WorkerConfiguration>();

        List<WorkerConfiguration> items = await GetAllIdleWorkersAsync();

        foreach (WorkerConfiguration item in items)
        {
            if (item.NextRunTime < DateTime.UtcNow)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public async Task<bool> DoRandomWorkAsync(WorkerActivityInformation info)
    {
        Random rand = new Random(DateTime.Now.Millisecond);
        // Worker will just pause here for some random number of minutes.
        if (rand.Next(1, 100) > 50)
        {
            // Success work.
            _logger.LogInformation("------------Expecting success------------");
            int max = _applicationSettings.MaximumIdleTimeInMinutes > 1 ?
                _applicationSettings.MaximumIdleTimeInMinutes - 1 : 1;
            int minutes = rand.Next(1, max);
            await Task.Delay(TimeSpan.FromMinutes(minutes));

            return true;
        }

        if (rand.Next(1, 100) > 50)
        {
            // Failure
            _logger.LogInformation("------------Expecting failure------------");
            await Task.Delay(TimeSpan.FromMinutes(_applicationSettings.MaximumIdleTimeInMinutes - 1));
            return false;
        }

        // Require termination
        _logger.LogWarning("------------Expecting termination------------");
        await Task.Delay(TimeSpan.FromMinutes(_applicationSettings.MaximumIdleTimeInMinutes + 2));
        return false;
    }

    public async Task SaveWorkerConfigurationAsync(WorkerConfiguration newWorkerConfig)
    {
        List<WorkerConfiguration> items = await GetAllWorkingItemsAsync();
        var workerConfig = items.FirstOrDefault(w => w.Id == newWorkerConfig.Id);
        if (workerConfig != null)
        {
            items.Remove(workerConfig);
        }

        items.Add(newWorkerConfig);
        _memoryCache.Set(WorkingItemsKey, items);
    }

    public async Task<WorkerConfiguration?> GetWorkerConfigurationByInstanceIdAsync(string instanceId)
    {
        List<WorkerConfiguration> items = await GetAllWorkingItemsAsync();
        var workerConfig = items.FirstOrDefault(w => w.InstanceId == instanceId);
        return workerConfig;
    }

    public async Task<WorkerConfiguration?> GetWorkerConfigurationByIdAsync(Guid id)
    {
        List<WorkerConfiguration> items = await GetAllWorkingItemsAsync();
        var workerConfig = items.FirstOrDefault(w => w.Id == id);
        return workerConfig;
    }
}