using Microsoft.Extensions.Options;

namespace HttpTriggerExample.Services;

public class MyExceptionCreatorService : IMyExceptionCreatorService
{
    private readonly ServiceOptions _options;

    public MyExceptionCreatorService(IOptions<ServiceOptions> options)
    {
        _options = options.Value;
    }

    public void CreateArgumentException(string message)
    {
        throw new ArgumentException(nameof(message), $"{message} (Run: {_options.RunInformation}), which is an ArgumentException exception!");
    }

    public void CreateArgumentNullException(string message)
    {
        throw new ArgumentNullException(nameof(message), $"{message} (Run: {_options.RunInformation}), which is an ArgumentNullException exception!");
    }

    public void CreateUnauthorizedAccessException(string message)
    {
        throw new UnauthorizedAccessException($"{message} (Run: {_options.RunInformation}), which is an UnauthorizedAccessException exception!");
    }

    public void CreateDivideByZeroException(string message)
    {
        throw new DivideByZeroException($"{message} (Run: {_options.RunInformation}), which is an DivideByZeroException exception!");
    }

}