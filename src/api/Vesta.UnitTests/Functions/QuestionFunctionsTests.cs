using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vesta.Application.DTOs;
using Vesta.Application.Interfaces;
using Vesta.Functions.Functions;
using Vesta.Functions.Models;
using Vesta.UnitTests.TestHelpers;
using Xunit;

namespace Vesta.UnitTests.Functions;

public class QuestionFunctionsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static QuestionFunctions CreateSut(out Mock<IQuestionService> questionService, out Mock<IUserService> userService)
    {
        questionService = new Mock<IQuestionService>();
        userService = new Mock<IUserService>();
        return new QuestionFunctions(questionService.Object, userService.Object, Mock.Of<ILogger<QuestionFunctions>>());
    }

    private static FakeFunctionContext ContextWithUser(System.Security.Claims.ClaimsPrincipal? user)
    {
        var context = new FakeFunctionContext();
        context.Items["User"] = user!;
        return context;
    }

    private static async Task<FunctionResponse<T>> ReadBody<T>(FakeHttpResponseData response) where T : class
    {
        var json = response.ReadBodyAsString();
        var result = JsonSerializer.Deserialize<FunctionResponse<T>>(json, JsonOptions);
        result.Should().NotBeNull();
        return result!;
    }

    private static UserDto AdminUser(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        DisplayName = "Admin Person",
        Roles = [new() { Id = Guid.NewGuid(), Name = "Admin" }],
        CreatedAt = DateTime.UtcNow
    };

    private static QuestionDto SampleQuestion(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Content = "What is your favorite color?",
        QuestionType = "MultipleChoice",
        Category = "Preferences",
        MinAge = 5,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        Options =
        [
            new() { Id = Guid.NewGuid(), Label = "Red", IsOther = false, SortOrder = 0 },
            new() { Id = Guid.NewGuid(), Label = "Blue", IsOther = false, SortOrder = 1 },
            new() { Id = Guid.NewGuid(), Label = "Green", IsOther = false, SortOrder = 2 },
            new() { Id = Guid.NewGuid(), Label = "Other", IsOther = true, SortOrder = 3 }
        ]
    };

    // ================= GetAllQuestions =================

    [Fact]
    public async Task GetAllQuestions_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _, out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllQuestions(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllQuestions_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(new UserDto
        {
            Id = Guid.NewGuid(),
            DisplayName = "Regular User",
            Roles = [new() { Id = Guid.NewGuid(), Name = "User" }],
            CreatedAt = DateTime.UtcNow
        });
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllQuestions(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllQuestions_ReturnsOk_WithAllQuestions_OnHappyPath()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var questions = new List<QuestionDto> { SampleQuestion(), SampleQuestion() };
        questionService.Setup(s => s.GetAllAsync()).ReturnsAsync(questions);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllQuestions(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<IList<QuestionDto>>(response);
        body.Success.Should().BeTrue();
        body.Data.Should().HaveCount(2);
    }

    // ================= CreateQuestion =================

    [Fact]
    public async Task CreateQuestion_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _, out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "POST");

        var response = (FakeHttpResponseData)await sut.CreateQuestion(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateQuestion_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(new UserDto
        {
            Id = Guid.NewGuid(),
            DisplayName = "Regular User",
            Roles = [new() { Id = Guid.NewGuid(), Name = "User" }],
            CreatedAt = DateTime.UtcNow
        });
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST");

        var response = (FakeHttpResponseData)await sut.CreateQuestion(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateQuestion_ReturnsBadRequest_WhenBodyIsMalformedJson()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{not-json");

        var response = (FakeHttpResponseData)await sut.CreateQuestion(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuestion_ReturnsBadRequest_WhenBodyIsNull()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "null");

        var response = (FakeHttpResponseData)await sut.CreateQuestion(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateQuestion_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        questionService.Setup(s => s.CreateAsync(It.IsAny<CreateQuestionRequest>()))
            .ThrowsAsync(new ArgumentException("Content is required."));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"content\":\"\",\"options\":[]}");

        var response = (FakeHttpResponseData)await sut.CreateQuestion(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ReadBody<QuestionDto>(response);
        body.Message.Should().Contain("Content");
    }

    [Fact]
    public async Task CreateQuestion_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var createdQuestion = SampleQuestion();
        questionService.Setup(s => s.CreateAsync(It.IsAny<CreateQuestionRequest>())).ReturnsAsync(createdQuestion);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"content\":\"Test Question\",\"questionType\":\"MultipleChoice\",\"options\":[]}");

        var response = (FakeHttpResponseData)await sut.CreateQuestion(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<QuestionDto>(response);
        body.Success.Should().BeTrue();
    }

    // ================= UpdateQuestion =================

    [Fact]
    public async Task UpdateQuestion_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _, out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "PUT");

        var response = (FakeHttpResponseData)await sut.UpdateQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateQuestion_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(new UserDto
        {
            Id = Guid.NewGuid(),
            DisplayName = "Regular User",
            Roles = [new() { Id = Guid.NewGuid(), Name = "User" }],
            CreatedAt = DateTime.UtcNow
        });
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT");

        var response = (FakeHttpResponseData)await sut.UpdateQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateQuestion_ReturnsBadRequest_WhenBodyIsMalformedJson()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{not-json");

        var response = (FakeHttpResponseData)await sut.UpdateQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateQuestion_ReturnsBadRequest_WhenBodyIsNull()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "null");

        var response = (FakeHttpResponseData)await sut.UpdateQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateQuestion_ReturnsNotFound_WhenServiceThrowsKeyNotFoundException()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        questionService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateQuestionRequest>()))
            .ThrowsAsync(new KeyNotFoundException("not found"));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{\"content\":\"Updated\",\"options\":[]}");

        var response = (FakeHttpResponseData)await sut.UpdateQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateQuestion_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var updatedQuestion = SampleQuestion();
        questionService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateQuestionRequest>())).ReturnsAsync(updatedQuestion);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{\"content\":\"Updated\",\"options\":[]}");

        var response = (FakeHttpResponseData)await sut.UpdateQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<QuestionDto>(response);
        body.Success.Should().BeTrue();
    }

    // ================= DeleteQuestion =================

    [Fact]
    public async Task DeleteQuestion_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _, out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteQuestion_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out _, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(new UserDto
        {
            Id = Guid.NewGuid(),
            DisplayName = "Regular User",
            Roles = [new() { Id = Guid.NewGuid(), Name = "User" }],
            CreatedAt = DateTime.UtcNow
        });
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteQuestion_ReturnsNotFound_WhenServiceThrowsKeyNotFoundException()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        questionService.Setup(s => s.DeactivateAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new KeyNotFoundException("not found"));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteQuestion_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var questionService, out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var deactivatedQuestion = SampleQuestion();
        questionService.Setup(s => s.DeactivateAsync(It.IsAny<Guid>())).ReturnsAsync(deactivatedQuestion);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteQuestion(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<QuestionDto>(response);
        body.Success.Should().BeTrue();
    }
}
