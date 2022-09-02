using HttpTriggerExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DotNetCoreNotIsolated.Functions;

public class FunctionUserException3
{
    private readonly IMyExceptionCreatorService _exceptionCreatorService;
    private readonly IExceptionHandlingService _exceptionHandlingService;

    public FunctionUserException3(IMyExceptionCreatorService exceptionCreatorService, IExceptionHandlingService exceptionHandlingService)
    {
        _exceptionCreatorService = exceptionCreatorService;
        _exceptionHandlingService = exceptionHandlingService;
    }

    [FunctionName("UserException3")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        try
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            _exceptionCreatorService.CreateUnauthorizedAccessException("This was is exception 3");

            return new OkResult();
        }
        catch (Exception ex)
        {
            return _exceptionHandlingService.HandleError(ex);
        }
    }
}