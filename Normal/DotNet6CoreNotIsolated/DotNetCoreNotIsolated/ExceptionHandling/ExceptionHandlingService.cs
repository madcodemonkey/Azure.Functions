using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace DotNetCoreNotIsolated;

public interface IExceptionHandlingService
{
    IActionResult HandleError(Exception ex);
}

public class ExceptionHandlingService : IExceptionHandlingService
{
    private readonly ILogger<ExceptionHandlingService> _logger;
    private readonly IHostingEnvironment _env;

    public ExceptionHandlingService(ILogger<ExceptionHandlingService> logger, IHostingEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public IActionResult HandleError(Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception encountered");
 
        MyExceptionHandlerMessage theMessageToReturn;
        if (ex is ArgumentException || ex is ArgumentNullException)
        {
            theMessageToReturn = new MyExceptionHandlerMessage(HttpStatusCode.BadRequest, ex.Message, "Some details here");

        }
        else if (ex is UnauthorizedAccessException)
        {
            theMessageToReturn = new MyExceptionHandlerMessage(HttpStatusCode.Unauthorized, ex.Message, "You are not allowed to do that!!!");

        }
        else
        {
             theMessageToReturn = _env.IsDevelopment() ?
                new MyExceptionHandlerMessage(HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace) :
                new MyExceptionHandlerMessage(HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        return new ObjectResult(theMessageToReturn)
        {
            StatusCode = (int) theMessageToReturn.StatusCode
        };
    }
}