using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Vesta.Functions.Functions;

public class QuestionFunctions
{
    private readonly ILogger<QuestionFunctions> _logger;

    public QuestionFunctions(ILogger<QuestionFunctions> logger)
    {
        _logger = logger;
    }

    [Function("QuestionFunctions")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}