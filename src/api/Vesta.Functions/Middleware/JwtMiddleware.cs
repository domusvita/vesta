using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using HttpRequestData = Microsoft.Azure.Functions.Worker.Http.HttpRequestData;

namespace Vesta.Functions.Middleware;

/// <summary>
/// Middleware to validate JWT tokens in Azure Functions.
/// </summary>
/// <param name="configuration"></param>
public class JwtMiddleware(IConfiguration configuration) : IFunctionsWorkerMiddleware
{
    /// <summary>
    /// Configuration manager for retrieving OpenID Connect configuration.
    /// </summary>
    private ConfigurationManager<OpenIdConnectConfiguration>? _configurationManager;

    // <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var httpContext = await context.GetHttpRequestDataAsync();
        if (httpContext == null)
        {
            await next(context);
            return;
        }
        
        if (string.Equals(httpContext.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            var optionsResponse = httpContext.CreateResponse(HttpStatusCode.NoContent);
            AddCorsHeaders(optionsResponse);
            context.GetInvocationResult().Value = optionsResponse;
            return;
        }

        var token = ExtractToken(httpContext);
        if (token == null)
        {
            await RespondUnauthorized(context, httpContext);
            return;
        }

        try
        {
            var principal = await ValidateTokenAsync(token);
            context.Items["User"] = principal;
        }
        catch
        {
            await RespondUnauthorized(context, httpContext);
            return;
        }

        await next(context);

        if (context.GetInvocationResult().Value is HttpResponseData response)
        {
            AddCorsHeaders(response);
        }

    /// <summary>
    /// Extracts the JWT token from the Authorization header of the HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request data.</param>
    /// <returns>The JWT token if present; otherwise, null.</returns>
    private static string? ExtractToken(HttpRequestData request)
    {
        if (!request.Headers.TryGetValues("Authorization", out var values))
        {
            return null;
        }

        var header = values.FirstOrDefault();
        if (header is null || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return header["Bearer ".Length..].Trim();
    }

    /// <summary>
    /// Responds with an Unauthorized status code and message if the JWT token is invalid or missing.
    /// </summary>
    /// <param name="context">The function context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task RespondUnauthorized(FunctionContext context, HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.Unauthorized);
        AddCorsHeaders(response);

        await response.WriteStringAsync("Unauthorized");
        context.GetInvocationResult().Value = response;
    }

    private static void AddCorsHeaders(HttpResponseData response)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5173");
        response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,PATCH,DELETE,OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Authorization,Content-Type");
        response.Headers.Add("Access-Control-Allow-Credentials", "true");
    }

    /// <summary>
    /// Validates the JWT token using Auth0's OpenID Connect configuration.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The claims principal if the token is valid.</returns>
    /// <exception cref="SecurityTokenValidationException">Thrown if the token is invalid.</exception>
    private async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
    {
        var domain = configuration["Auth0:Domain"];
        var audience = configuration["Auth0:Audience"];
        var authority = $"https://{domain}/";

        _configurationManager ??= new ConfigurationManager<OpenIdConnectConfiguration>
        (
            $"{authority}.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever()
        );

        var openIdConfig = await _configurationManager.GetConfigurationAsync();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = authority,
            ValidAudience = audience,
            IssuerSigningKeys = openIdConfig.SigningKeys,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        var handler = new JsonWebTokenHandler();
        var result = await handler.ValidateTokenAsync(token, validationParameters);
        return !result.IsValid
            ? throw new SecurityTokenValidationException("Invalid token")
            : new ClaimsPrincipal(result.ClaimsIdentity);
    }
}