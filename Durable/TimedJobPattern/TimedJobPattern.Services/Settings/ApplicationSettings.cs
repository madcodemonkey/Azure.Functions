namespace TimedJobPattern.Services;

public class ApplicationSettings 
{
    public const string SectionName = "AppSetting";
    
    /// <summary>
    /// The maximum number of minutes before a orchestrator is considered idle and is killed.
    /// </summary>
    public int MaximumIdleTimeInMinutes { get; set; } = 15;
}
 