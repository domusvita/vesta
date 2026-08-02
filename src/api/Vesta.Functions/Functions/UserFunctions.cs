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
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        var users = await userService.GetAllAsync();
        return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<IList<UserDto>>(true, users));
    }

    /// <summary>
    /// Handles the HTTP PUT request to update an existing user's profile. Restricted to Admin users.
    /// </summary>
    [Function("UpdateUser")]
    public async Task<HttpResponseData> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "users/{id:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid id
    )
    {
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        UpsertUserRequest? request;
        try
        {
            request = await System.Text.Json.JsonSerializer.DeserializeAsync<UpsertUserRequest>(
                req.Body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false));
        }

        if (request is null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false));
        }

        try
        {
            var updatedUser = await userService.UpdateAsync(id, request);
            return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<UserDto>(true, updatedUser));
        }
        catch (ArgumentException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false, message: ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.NotFound, new FunctionResponse<UserDto>(false, message: ex.Message));
        }
    }

    /// <summary>
    /// Handles the HTTP POST request for an Admin to create a new user profile without an Auth0 identity.
    /// </summary>
    [Function("CreateUserByAdmin")]
    public async Task<HttpResponseData> CreateUserByAdmin(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users/admin")] HttpRequestData req,
        FunctionContext context
    )
    {
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        UpsertUserRequest? request;
        try
        {
            request = await System.Text.Json.JsonSerializer.DeserializeAsync<UpsertUserRequest>(
                req.Body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false));
        }

        if (request is null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false));
        }

        try
        {
            var createdUser = await userService.CreateByAdminAsync(request);
            return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<UserDto>(true, createdUser));
        }
        catch (ArgumentException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<UserDto>(false, message: ex.Message));
        }
    }

    /// <summary>
    /// Handles the HTTP DELETE request to remove all role assignments from a user. Restricted to Admin users.
    /// </summary>
    [Function("DeleteUser")]
    public async Task<HttpResponseData> DeleteUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "users/{id:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid id
    )
    {
        var (adminUser, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        if (adminUser!.Id.HasValue && adminUser.Id.Value == id)
        {
            return await CreateJsonResponse(
                req,
                HttpStatusCode.BadRequest,
                new FunctionResponse<UserDto>(false, message: "You cannot remove your own roles.")
            );
        }

        try
        {
            var updatedUser = await userService.RemoveAllRolesAsync(id);
            return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<UserDto>(true, updatedUser));
        }
        catch (KeyNotFoundException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.NotFound, new FunctionResponse<UserDto>(false, message: ex.Message));
        }
    }

    /// <summary>
    /// Resolves the calling user and verifies they hold the Admin role.
    /// </summary>
    /// <returns>
    /// The current user's <see cref="UserDto"/> and a null error response on success; otherwise a null
    /// user and an Unauthorized/Forbidden response to return directly to the caller.
    /// </returns>
    private async Task<(UserDto? User, HttpResponseData? ErrorResponse)> RequireAdminAsync(
        HttpRequestData req,
        FunctionContext context
    )
    {
        var user = context.Items["User"] as ClaimsPrincipal;
        var auth0Id = GetAuth0Id(user);

        if (string.IsNullOrEmpty(auth0Id))
        {
            return (null, await CreateJsonResponse(req, HttpStatusCode.Unauthorized, new FunctionResponse<object>(false)));
        }

        var currentUser = await userService.GetByAuthIdAsync(auth0Id);
        if (currentUser is null || !currentUser.Roles.Contains("Admin"))
        {
            return (null, await CreateJsonResponse(req, HttpStatusCode.Forbidden, new FunctionResponse<object>(false)));
        }

        return (currentUser, null);
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