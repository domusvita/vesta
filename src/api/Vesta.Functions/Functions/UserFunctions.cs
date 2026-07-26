using System.Security.Claims;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Vesta.Application.DTOs;
using Vesta.Application.Interfaces;
using Vesta.Functions.Models;

namespace Vesta.Functions.Functions;

/// <summary>
/// Represents the Azure Functions related to user operations.
/// </summary>
/// <param name="userService">The service for managing user-related operations.</param>
/// <param name="logger">The logger for logging information and errors.</param>
public class UserFunctions(IUserService userService, ILogger<UserFunctions> logger)
{
    /// <summary>
    /// Handles the HTTP GET request to retrieve the current user's information.
    /// </summary>
    [Function("GetCurrentUser")]
    public async Task<HttpResponseData> GetCurrentUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/me")] HttpRequestData req,
        FunctionContext context
    )
    {
        var user = context.Items["User"] as ClaimsPrincipal;
        var auth0Id = GetAuth0Id(user);

        if (string.IsNullOrEmpty(auth0Id))
        {
            return await CreateJsonResponse(req, HttpStatusCode.Unauthorized, new FunctionResponse<UserDto>(false));
        }

        var userDto = await userService.GetByAuthIdAsync(auth0Id);
        if (userDto is null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.NotFound, new FunctionResponse<UserDto>(false));
        }

        return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<UserDto>(true, userDto));
    }

    /// <summary>
    /// Handles the HTTP POST request to register a new user on first login.
    /// </summary>
    [Function("RegisterUser")]
    public async Task<HttpResponseData> RegisterUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/register")] HttpRequestData req,
        FunctionContext context
    )
    {
        var user = context.Items["User"] as ClaimsPrincipal;
        var auth0Id = GetAuth0Id(user);

        if (string.IsNullOrEmpty(auth0Id))
        {
            return await CreateJsonResponse(req, HttpStatusCode.Unauthorized, new FunctionResponse<UserDto>(false));
        }

        CreateUserRequest? request;
        try
        {
            request = await System.Text.Json.JsonSerializer.DeserializeAsync<CreateUserRequest>(
                req.Body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false));
        }

        if (request is null || string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false));
        }

        var existingUser = await userService.GetByAuthIdAsync(auth0Id);
        if (existingUser is not null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.Conflict, new FunctionResponse<UserDto>(false));
        }

        var userDto = await userService.CreateAsync(auth0Id, request.DisplayName);
        return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<UserDto>(true, userDto));
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve all users. Restricted to Admin users.
    /// </summary>
    [Function("GetAllUsers")]
    public async Task<HttpResponseData> GetAllUsers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequestData req,
        FunctionContext context
    )
    {
        var user = context.Items["User"] as ClaimsPrincipal;
        var auth0Id = GetAuth0Id(user);

        if (string.IsNullOrEmpty(auth0Id))
        {
            return await CreateJsonResponse(req, HttpStatusCode.Unauthorized, new FunctionResponse<IList<UserDto>>(false));
        }

        var currentUser = await userService.GetByAuthIdAsync(auth0Id);
        if (currentUser is null || !currentUser.Roles.Contains("Admin"))
        {
            return await CreateJsonResponse(req, HttpStatusCode.Forbidden, new FunctionResponse<IList<UserDto>>(false));
        }

        var users = await userService.GetAllAsync();
        return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<IList<UserDto>>(true, users));
    }

    private static async Task<HttpResponseData> CreateJsonResponse<T>(
        HttpRequestData req,
        HttpStatusCode statusCode,
        T body
    )
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(body);
        return response;
    }

    private static string? GetAuth0Id(ClaimsPrincipal? user)
    {
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user?.FindFirst("sub")?.Value;
    }
}