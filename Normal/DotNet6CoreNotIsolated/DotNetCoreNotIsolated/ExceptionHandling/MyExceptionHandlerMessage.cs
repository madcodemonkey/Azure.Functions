using System.Net;

namespace DotNetCoreNotIsolated;

public class MyExceptionHandlerMessage
{
    /// <summary>Constructor</summary>
    public MyExceptionHandlerMessage() { }

    /// <summary>Constructor</summary>
    public MyExceptionHandlerMessage(HttpStatusCode statusCode, string? message = null, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
    
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Details { get; set; }
}