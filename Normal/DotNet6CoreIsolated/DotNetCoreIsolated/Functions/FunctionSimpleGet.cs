using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DotNetCoreIsolated;

public class FunctionSimpleGet
{
    private readonly ILogger _logger;

    public FunctionSimpleGet(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FunctionSimpleGet>();
    }

    [Function("SimpleGet")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext, string? greeting, string? lastname)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var output = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"Welcome to Azure Functions! Parameter: {greeting ?? "Unspecified greeting"}, {lastname ?? "Unknown"}" +
            $" Parsed Query string:  {output["greeting"] ?? "Unspecified greeting"}, {output["lastname"] ?? "Unknown"}");

        return response;
    }
}