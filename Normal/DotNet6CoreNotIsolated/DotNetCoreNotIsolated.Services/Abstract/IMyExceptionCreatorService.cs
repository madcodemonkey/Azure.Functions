namespace DotNetCoreNotIsolated.Services;

public interface IMyExceptionCreatorService
{
    void CreateArgumentException(string message);
    void CreateArgumentNullException(string message);
    void CreateUnauthorizedAccessException(string message);
    void CreateDivideByZeroException(string message);
}