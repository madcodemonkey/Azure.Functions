using HttpTriggerExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DotNetCoreIsolated;

public class FunctionUserException2
{
    private readonly IMyExceptionCreatorService _exceptionCreatorService;
    private readonly ILogger _logger;

    public FunctionUserException2(ILoggerFactory loggerFactory, IMyExceptionCreatorService exceptionCreatorService)
    {
        _exceptionCreatorService = exceptionCreatorService;
        _logger = loggerFactory.CreateLogger<FunctionSimpleGet>();
    }

    [Function("UserException2")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        _exceptionCreatorService.CreateArgumentNullException("This was is exception 2");

        return req.CreateResponse(HttpStatusCode.OK);
    }

}