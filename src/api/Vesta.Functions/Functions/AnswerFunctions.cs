using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Vesta.Functions.Functions;

public class AnswerFunctions
{
    private readonly ILogger<AnswerFunctions> _logger;

    public AnswerFunctions(ILogger<AnswerFunctions> logger)
    {
        _logger = logger;
    }

    [Function("AnswerFunctions")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}