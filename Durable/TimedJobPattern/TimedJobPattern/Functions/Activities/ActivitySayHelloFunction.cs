using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TimedJobPattern;

public class ActivitySayHelloFunction
{

    [Function(nameof(SayHello))]
    public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(SayHello));
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }

 
}