namespace TimedJobPattern.Modals;

public class WorkerConfiguration
{
    public Guid Id { get; set; }

    /// <summary>
    /// The orchestration id.  This is only set when the worker is running.
    /// </summary>
    public string? InstanceId { get; set; }
  
    /// <summary>
    /// If true, it indicates that you want to add the <see cref="RunTimeIntervalInMinutes"/> to
    /// NextRunTime till it corresponds to sometime in the future. If false, it means you want the
    /// <see cref="RunTimeIntervalInMinutes"/> added to DateTime.UtcNow; thus, the time could shift
    /// based upon how long it takes to update the Azure Cognitive Search Index.
    /// </summary>
    public bool AddIntervalToNextRunTime { get; set; } = true;


    /// <summary>
    /// The last time that the worker ran.
    /// </summary>
    public DateTime? LastRunTime { get; set; }

    /// <summary>
    /// When running, this is the last time the worker saved a heartbeat status.
    /// </summary>
    public DateTime? LastHeartbeatTime { get; set; }


    /// <summary>
    /// The next time that the worker should run in UTC time. If this time has passed, the worker
    /// will run immediately.
    /// </summary>
    public DateTime NextRunTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The interval between run times. This is how much time will be added to the current utc time
    /// when processing has finished. This is not used if <see cref="AddIntervalToNextRunTime"/> is
    /// set to true.
    /// </summary>
    public int RunTimeIntervalInMinutes { get; set; } = 60;
}