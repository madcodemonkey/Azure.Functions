using HttpTriggerExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DotNetCoreIsolated;

public class FunctionUserException4
{
    private readonly IMyExceptionCreatorService _exceptionCreatorService;
    private readonly ILogger _logger;

    public FunctionUserException4(ILoggerFactory loggerFactory, IMyExceptionCreatorService exceptionCreatorService)
    {
        _exceptionCreatorService = exceptionCreatorService;
        _logger = loggerFactory.CreateLogger<FunctionSimpleGet>();
    }

    [Function("UserException4")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        _exceptionCreatorService.CreateDivideByZeroException("This was is exception 4");

        return req.CreateResponse(HttpStatusCode.OK);
    }

}