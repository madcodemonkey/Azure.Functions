namespace HttpTriggerExample.Services;

public class ServiceOptions
{
    public static string SectionName = "ServiceInfo";

    public string? RunInformation { get; set; }

    public string? Greeting { get; set; }
}