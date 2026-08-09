using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Vesta.Application.DTOs;
using Vesta.Application.Interfaces;
using Vesta.Functions.Models;

namespace Vesta.Functions.Functions;

/// <summary>
/// Represents the Azure Functions related to question operations. Restricted to Admin users only.
/// </summary>
/// <param name="questionService">The service for managing question-related operations.</param>
/// <param name="userService">The service for managing user-related operations.</param>
/// <param name="logger">The logger for logging information and errors.</param>
public class QuestionFunctions(IQuestionService questionService, IUserService userService, ILogger<QuestionFunctions> logger)
{
    /// <summary>
    /// Handles the HTTP GET request to retrieve all active questions. Restricted to Admin users.
    /// </summary>
    [Function("GetAllQuestions")]
    public async Task<HttpResponseData> GetAllQuestions(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questions")] HttpRequestData req,
        FunctionContext context
    )
    {
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        var questions = await questionService.GetAllAsync();
        return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<IList<QuestionDto>>(true, questions));
    }

    /// <summary>
    /// Handles the HTTP POST request to create a new question. Restricted to Admin users.
    /// </summary>
    [Function("CreateQuestion")]
    public async Task<HttpResponseData> CreateQuestion(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "questions")] HttpRequestData req,
        FunctionContext context
    )
    {
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        CreateQuestionRequest? request;
        try
        {
            request = await System.Text.Json.JsonSerializer.DeserializeAsync<CreateQuestionRequest>(
                req.Body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<QuestionDto>(false));
        }

        if (request is null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<QuestionDto>(false));
        }

        try
        {
            var createdQuestion = await questionService.CreateAsync(request);
            return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<QuestionDto>(true, createdQuestion));
        }
        catch (ArgumentException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<QuestionDto>(false, message: ex.Message));
        }
    }

    /// <summary>
    /// Handles the HTTP PUT request to update an existing question. Restricted to Admin users.
    /// </summary>
    [Function("UpdateQuestion")]
    public async Task<HttpResponseData> UpdateQuestion(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "questions/{id:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid id
    )
    {
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        UpdateQuestionRequest? request;
        try
        {
            request = await System.Text.Json.JsonSerializer.DeserializeAsync<UpdateQuestionRequest>(
                req.Body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<QuestionDto>(false));
        }

        if (request is null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<QuestionDto>(false));
        }

        try
        {
            var updatedQuestion = await questionService.UpdateAsync(id, request);
            return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<QuestionDto>(true, updatedQuestion));
        }
        catch (ArgumentException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new FunctionResponse<QuestionDto>(false, message: ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.NotFound, new FunctionResponse<QuestionDto>(false, message: ex.Message));
        }
    }

    /// <summary>
    /// Handles the HTTP DELETE request to soft-delete a question by setting IsActive to false. Restricted to Admin users.
    /// </summary>
    [Function("DeleteQuestion")]
    public async Task<HttpResponseData> DeleteQuestion(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "questions/{id:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid id
    )
    {
        var (_, errorResponse) = await RequireAdminAsync(req, context);
        if (errorResponse is not null)
        {
            return errorResponse;
        }

        try
        {
            var deletedQuestion = await questionService.DeactivateAsync(id);
            return await CreateJsonResponse(req, HttpStatusCode.OK, new FunctionResponse<QuestionDto>(true, deletedQuestion));
        }
        catch (KeyNotFoundException ex)
        {
            return await CreateJsonResponse(req, HttpStatusCode.NotFound, new FunctionResponse<QuestionDto>(false, message: ex.Message));
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
        if (currentUser is null || !currentUser.Roles.Any(r => r.Name == "Admin"))
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