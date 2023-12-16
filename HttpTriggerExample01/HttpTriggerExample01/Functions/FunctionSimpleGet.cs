using System.Collections.Specialized;
using System.Net;
using HttpTriggerExample.Model; 
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpTriggerExample.Functions;

public class FunctionSimpleGet
{
    private readonly ExampleOptions _exampleOptions;
    private readonly ILogger _logger;

    public FunctionSimpleGet(ILoggerFactory loggerFactory, IOptions<ExampleOptions> exampleOptions)
    {
        _exampleOptions = exampleOptions.Value; 
        _logger = loggerFactory.CreateLogger<FunctionSimpleGet>();
    }

    [Function("FunctionSimpleGet")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext, string? greeting ="hi", string? lastname = "John Doe")
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Getting query parameters (Way 1) - They are injected (see parameters above)

        // Getting query parameters (Way 2)
        NameValueCollection output = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString($"Welcome to Azure Functions! {_exampleOptions.SomeNonNestedSetting}  " +
                             $"| Parameter: {greeting ?? "Unspecified greeting"}, {lastname ?? "Unknown"} |||" +
                             $"| Parsed Query string:  {output["greeting"] ?? "Unspecified greeting"}, {output["lastname"] ?? "Unspecified lastname"}");

        return response;
    }
}