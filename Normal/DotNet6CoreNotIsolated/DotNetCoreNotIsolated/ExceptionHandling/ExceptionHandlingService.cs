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
        // Even though you can find the exceptions error message in the Application Log Exceptions area, the trace area does NOT
        // give you the error message, it will only print out the text we put as the 2nd argument to this LogError method call,
        // so let's log something useful in the trace log if they don't know to look in the error area for a stack trace.
        _logger.LogError(ex, $"Unhandled exception encountered: {ex.Message}");
 
        MyExceptionHandlerMessage exceptionMessage = WrapTheException(ex);

        return new ObjectResult(exceptionMessage)
        {
            StatusCode = (int) exceptionMessage.StatusCode
        };
    }

    /// <summary>Creates a wrapper around the exception which we will serialize and send to the caller.</summary>
    /// <param name="ex">The original exception.</param>
    private MyExceptionHandlerMessage WrapTheException(Exception ex)
    {
        MyExceptionHandlerMessage result;
        if (ex is ArgumentException || ex is ArgumentNullException)
        {
            result = new MyExceptionHandlerMessage(HttpStatusCode.BadRequest, ex.Message, "Some details here");
        }
        else if (ex is UnauthorizedAccessException)
        {
            result = new MyExceptionHandlerMessage(HttpStatusCode.Unauthorized, ex.Message, "You are not allowed to do that!!!");
        }
        else
        {
            result = _env.IsDevelopment() ? 
                new MyExceptionHandlerMessage(HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace) : 
                new MyExceptionHandlerMessage(HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        return result;
    }
}