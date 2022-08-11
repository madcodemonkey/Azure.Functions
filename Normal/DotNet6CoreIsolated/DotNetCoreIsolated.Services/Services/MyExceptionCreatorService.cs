namespace DotNetCoreIsolated.Services;

public class MyExceptionCreatorService : IMyExceptionCreatorService
{
    public void CreateArgumentException(string message)
    {
        throw new ArgumentException(nameof(message), $"{message}, which is an ArgumentException exception!");
    }

    public void CreateArgumentNullException(string message)
    {
        throw new ArgumentNullException(nameof(message), $"{message}, which is an ArgumentNullException exception!");
    }

    public void CreateUnauthorizedAccessException(string message)
    {
        throw new UnauthorizedAccessException($"{message}, which is an UnauthorizedAccessException exception!");
    }

    public void CreateDivideByZeroException(string message)
    {
        throw new DivideByZeroException($"{message}, which is an DivideByZeroException exception!");
    }

}