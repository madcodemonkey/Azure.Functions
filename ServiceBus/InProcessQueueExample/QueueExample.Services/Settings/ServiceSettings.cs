namespace QueueExample.Services;

public class ServiceSettings
{
    public string QueueConnectionString { get; set; } = null!;
    public string QueueName { get; set; } = null!;
    public string QueueFullyQualifiedNamespace { get; set; } = null!;
}