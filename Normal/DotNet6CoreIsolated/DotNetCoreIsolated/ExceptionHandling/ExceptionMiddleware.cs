using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DotNetCoreIsolated;

/// <summary>A middleware class designed to handle uncaught errors.</summary>
/// <remarks>This middleware is added in the program.cs with this line: app.UseMiddleware<ExceptionMiddleware>() </remarks>
public class ExceptionMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    /// <summary>Constructor</summary>
    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    /// <summary>This method is called as part of the pipeline.  We are just passing it along to the next thing
    /// in the pipeline, but if an uncaught error occurs, it should bubble all the way back to here and get
    /// interpreted by this code.  If it's a user error, we return the appropriate code; otherwise, we cover it
    /// up with an internal 500 error so that things aren't revealed to the caller.  The only exception is when
    /// we are in DEV mode, we bubble the exception and stack trace back to the caller.</summary>
    /// <param name="context">The way you access information about the execution context.</param>
    /// <param name="next">Used to execute the next item in the pipeline</param>
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            // Code before function execution here
            await next(context);
            // Code after function execution here
        }
        catch (Exception ex)
        {
            // Even though you can find the exceptions error message in the Application Log Exceptions area, the trace area does NOT
            // give you the error message, it will only print out the text we put as the 2nd argument to this LogError method call,
            // so let's log something useful in the trace log if they don't know to look in the error area for a stack trace.
            _logger.LogError(ex, $"Unhandled exception encountered: {ex.Message}");

            // https://stackoverflow.com/a/73080248/97803
            // Requires Microsoft.Azure.Functions.Worker NuGet package of 1.8.0 to use the GetHttpRequestDataAsync extension method!
            var req = await context.GetHttpRequestDataAsync();
            if (req == null)
                throw;

            MyExceptionHandlerMessage exceptionMessage = WrapTheException(ex);

            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(exceptionMessage, exceptionMessage.StatusCode);  // Found that you have to set statusCode here!
            context.GetInvocationResult().Value = response;
        }
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