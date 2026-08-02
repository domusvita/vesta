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

public class UserFunctionsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static UserFunctions CreateSut(out Mock<IUserService> userService)
    {
        userService = new Mock<IUserService>();
        return new UserFunctions(userService.Object, Mock.Of<ILogger<UserFunctions>>());
    }

    private static FakeFunctionContext ContextWithUser(System.Security.Claims.ClaimsPrincipal? user)
    {
        var context = new FakeFunctionContext();
        // In production, JwtMiddleware always either sets Items["User"] to a validated
        // principal or short-circuits the pipeline before the function runs, so the key is
        // always present by the time a function body executes. We replicate that here by
        // always setting the key (possibly to null) rather than leaving it absent.
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
        Roles = ["Admin"],
        CreatedAt = DateTime.UtcNow
    };

    private static UserDto RegularUser(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        DisplayName = "Regular Person",
        Roles = ["User"],
        CreatedAt = DateTime.UtcNow
    };

    // ================= GetCurrentUser =================

    [Fact]
    public async Task GetCurrentUser_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetCurrentUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await ReadBody<UserDto>(response)).Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsUnauthorized_WhenPrincipalHasNoIdClaim()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(TestPrincipalFactory.WithNoIdClaim());
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetCurrentUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync((UserDto?)null);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetCurrentUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsOk_WithUserDto_OnHappyPath()
    {
        var sut = CreateSut(out var userService);
        var dto = RegularUser();
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(dto);
        var context = ContextWithUser(TestPrincipalFactory.WithSubClaim("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetCurrentUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<UserDto>(response);
        body.Success.Should().BeTrue();
        body.Data!.DisplayName.Should().Be("Regular Person");
    }

    // ================= RegisterUser =================

    [Fact]
    public async Task RegisterUser_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"displayName\":\"X\"}");

        var response = (FakeHttpResponseData)await sut.RegisterUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterUser_ReturnsBadRequest_WhenBodyIsMalformedJson()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{not-json");

        var response = (FakeHttpResponseData)await sut.RegisterUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_ReturnsBadRequest_WhenDisplayNameMissing()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"displayName\":\"\"}");

        var response = (FakeHttpResponseData)await sut.RegisterUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_ReturnsConflict_WhenUserAlreadyExists()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(RegularUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"displayName\":\"New Name\"}");

        var response = (FakeHttpResponseData)await sut.RegisterUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        userService.Verify(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync((UserDto?)null);
        var created = RegularUser();
        userService.Setup(s => s.CreateAsync("auth0|1", "New Name")).ReturnsAsync(created);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"displayName\":\"New Name\"}");

        var response = (FakeHttpResponseData)await sut.RegisterUser(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<UserDto>(response);
        body.Success.Should().BeTrue();
    }

    // ================= GetAllUsers =================

    [Fact]
    public async Task GetAllUsers_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllUsers(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(RegularUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllUsers(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsForbidden_WhenCurrentUserNotFound()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync((UserDto?)null);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllUsers(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOk_WithAllUsers_OnHappyPath()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        userService.Setup(s => s.GetAllAsync()).ReturnsAsync([RegularUser(), AdminUser()]);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "GET");

        var response = (FakeHttpResponseData)await sut.GetAllUsers(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<IList<UserDto>>(response);
        body.Data.Should().HaveCount(2);
    }

    // ================= UpdateUser =================

    [Fact]
    public async Task UpdateUser_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "PUT");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(RegularUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenBodyIsMalformedJson()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{not-json");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenBodyIsNull()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "null");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        userService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpsertUserRequest>()))
            .ThrowsAsync(new ArgumentException("DisplayName is required and must be between 2 and 100 characters."));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{\"displayName\":\"A\"}");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ReadBody<UserDto>(response);
        body.Message.Should().Contain("DisplayName");
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenServiceThrowsKeyNotFoundException()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        userService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpsertUserRequest>()))
            .ThrowsAsync(new KeyNotFoundException("not found"));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{\"displayName\":\"Valid Name\"}");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var updated = RegularUser();
        userService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpsertUserRequest>())).ReturnsAsync(updated);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "PUT", bodyJson: "{\"displayName\":\"Valid Name\"}");

        var response = (FakeHttpResponseData)await sut.UpdateUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ================= CreateUserByAdmin =================

    [Fact]
    public async Task CreateUserByAdmin_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "POST");

        var response = (FakeHttpResponseData)await sut.CreateUserByAdmin(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateUserByAdmin_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(RegularUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST");

        var response = (FakeHttpResponseData)await sut.CreateUserByAdmin(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserByAdmin_ReturnsBadRequest_WhenBodyIsMalformedJson()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{not-json");

        var response = (FakeHttpResponseData)await sut.CreateUserByAdmin(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserByAdmin_ReturnsBadRequest_WhenBodyIsNull()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "null");

        var response = (FakeHttpResponseData)await sut.CreateUserByAdmin(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserByAdmin_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        userService.Setup(s => s.CreateByAdminAsync(It.IsAny<UpsertUserRequest>()))
            .ThrowsAsync(new ArgumentException("AvatarUrl must be a well-formed absolute http or https URL."));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"displayName\":\"Valid Name\",\"avatarUrl\":\"bad\"}");

        var response = (FakeHttpResponseData)await sut.CreateUserByAdmin(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUserByAdmin_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var created = RegularUser();
        userService.Setup(s => s.CreateByAdminAsync(It.IsAny<UpsertUserRequest>())).ReturnsAsync(created);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "POST", bodyJson: "{\"displayName\":\"Valid Name\"}");

        var response = (FakeHttpResponseData)await sut.CreateUserByAdmin(req, context);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ================= DeleteUser =================

    [Fact]
    public async Task DeleteUser_ReturnsUnauthorized_WhenNoUser()
    {
        var sut = CreateSut(out _);
        var context = ContextWithUser(null);
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteUser_ReturnsForbidden_WhenNotAdmin()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(RegularUser());
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteUser(req, context, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteUser_ReturnsBadRequest_WhenSelfLockoutAttempted()
    {
        var sut = CreateSut(out var userService);
        var adminId = Guid.NewGuid();
        var admin = AdminUser(adminId);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(admin);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteUser(req, context, adminId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await ReadBody<UserDto>(response);
        body.Message.Should().Contain("cannot remove your own roles");
        userService.Verify(s => s.RemoveAllRolesAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenTargetUserMissing()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var targetId = Guid.NewGuid();
        userService.Setup(s => s.RemoveAllRolesAsync(targetId)).ThrowsAsync(new KeyNotFoundException("not found"));
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteUser(req, context, targetId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_ReturnsOk_OnHappyPath()
    {
        var sut = CreateSut(out var userService);
        userService.Setup(s => s.GetByAuthIdAsync("auth0|1")).ReturnsAsync(AdminUser());
        var targetId = Guid.NewGuid();
        var updated = RegularUser(targetId);
        userService.Setup(s => s.RemoveAllRolesAsync(targetId)).ReturnsAsync(updated);
        var context = ContextWithUser(TestPrincipalFactory.WithAuth0Id("auth0|1"));
        var req = new FakeHttpRequestData(context, "DELETE");

        var response = (FakeHttpResponseData)await sut.DeleteUser(req, context, targetId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await ReadBody<UserDto>(response);
        body.Data!.Roles.Should().Contain("User");
    }
}
