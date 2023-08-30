namespace TimedJobPattern.Modals;

public class WorkerActivityInformation
{
    public Guid WorkerId { get; set; }
    public string InstanceId { get; set; } = string.Empty;
}