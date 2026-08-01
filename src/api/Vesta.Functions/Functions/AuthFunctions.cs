using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Vesta.Functions.Functions;

public class AuthFunctions
{
    private readonly ILogger<AuthFunctions> _logger;

    public AuthFunctions(ILogger<AuthFunctions> logger)
    {
        _logger = logger;
    }

    [Function("AuthFunctions")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}