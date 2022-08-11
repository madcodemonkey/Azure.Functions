using System;
using System.Threading.Tasks;
using DotNetCoreNotIsolated.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DotNetCoreNotIsolated.Functions;

public class FunctionUserException2
{
    private readonly IMyExceptionCreatorService _exceptionCreatorService;
    private readonly IExceptionHandlingService _exceptionHandlingService;

    public FunctionUserException2(IMyExceptionCreatorService exceptionCreatorService, IExceptionHandlingService exceptionHandlingService)
    {
        _exceptionCreatorService = exceptionCreatorService;
        _exceptionHandlingService = exceptionHandlingService;
    }

    [FunctionName("UserException2")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        try
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            _exceptionCreatorService.CreateArgumentNullException("This was is exception 2");

            return new OkResult();
        }
        catch (Exception ex)
        {
            return _exceptionHandlingService.HandleError(ex);
        }
    }
}