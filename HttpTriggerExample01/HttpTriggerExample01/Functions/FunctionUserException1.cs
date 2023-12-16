using System.Net;
using HttpTriggerExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HttpTriggerExample.Functions;

public class FunctionUserException1
{
    private readonly IMyExceptionCreatorService _exceptionCreatorService;
    private readonly ILogger _logger;

    public FunctionUserException1(ILoggerFactory loggerFactory, IMyExceptionCreatorService exceptionCreatorService)
    {
        _exceptionCreatorService = exceptionCreatorService;
        _logger = loggerFactory.CreateLogger<FunctionUserException1>();
    }

    [Function("UserException1")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request for Exception 1.");
         
        _exceptionCreatorService.CreateArgumentException("This was is exception 1");

        return req.CreateResponse(HttpStatusCode.OK);
    }
}