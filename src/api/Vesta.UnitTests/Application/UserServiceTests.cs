using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vesta.Application.DTOs;
using Vesta.Application.Services;
using Vesta.Domain.Entities;
using Vesta.Infrastructure.Persistence;
using Xunit;

namespace Vesta.UnitTests.Application;

public class UserServiceTests
{
    private static VestaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VestaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VestaDbContext(options);
    }

    private static UserEntity NewUser(string? auth0Id = "auth0|123", string displayName = "Alice Example")
    {
        return new UserEntity
        {
            Id = Guid.NewGuid(),
            Auth0Id = auth0Id,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // ---------- GetByAuthIdAsync ----------

    [Fact]
    public async Task GetByAuthIdAsync_ReturnsDto_WhenUserExists()
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.GetByAuthIdAsync("auth0|123");

        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Alice Example");
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByAuthIdAsync_ReturnsNull_WhenUserNotFound()
    {
        await using var db = CreateContext();
        var service = new UserService(db);

        var result = await service.GetByAuthIdAsync("auth0|does-not-exist");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByAuthIdAsync_ReturnsNull_WhenAuthIdIsNullAndNoMatchingUser()
    {
        await using var db = CreateContext();
        db.Users.Add(NewUser());
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.GetByAuthIdAsync(null);

        result.Should().BeNull();
    }

    // ---------- CreateAsync ----------

    [Fact]
    public async Task CreateAsync_AssignsExactlyOneUserRole_AndSetsTimestamps()
    {
        await using var db = CreateContext();
        var service = new UserService(db);
        var before = DateTime.UtcNow;

        var dto = await service.CreateAsync("auth0|new", "New Person");

        dto.DisplayName.Should().Be("New Person");
        dto.Roles.Should().ContainSingle().Which.Should().Be("User");
        dto.CreatedAt.Should().BeOnOrAfter(before);

        var stored = await db.Users.Include(u => u.Roles).FirstAsync(u => u.Auth0Id == "auth0|new");
        stored.Roles.Should().ContainSingle();
        stored.CreatedAt.Should().BeCloseTo(stored.UpdatedAt, TimeSpan.FromSeconds(1));
    }

    // ---------- GetAllAsync ----------

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers_OrderedByCreatedAt()
    {
        await using var db = CreateContext();
        var older = NewUser(auth0Id: "a1", displayName: "Older");
        older.CreatedAt = DateTime.UtcNow.AddDays(-2);
        var newer = NewUser(auth0Id: "a2", displayName: "Newer");
        newer.CreatedAt = DateTime.UtcNow.AddDays(-1);

        db.Users.AddRange(newer, older);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].DisplayName.Should().Be("Older");
        result[1].DisplayName.Should().Be("Newer");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsers()
    {
        await using var db = CreateContext();
        var service = new UserService(db);

        var result = await service.GetAllAsync();

        result.Should().BeEmpty();
    }

    // ---------- UpdateAsync ----------

    [Fact]
    public async Task UpdateAsync_UpdatesAllFields_AndUpdatedAt()
    {
        await using var db = CreateContext();
        var user = NewUser();
        user.UpdatedAt = DateTime.UtcNow.AddDays(-1);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var request = new UpsertUserRequest
        {
            DisplayName = "  Updated Name  ",
            DateOfBirth = new DateTime(1990, 1, 1),
            AvatarUrl = "https://example.com/avatar.png"
        };

        var result = await service.UpdateAsync(user.Id, request);

        result.DisplayName.Should().Be("Updated Name");
        result.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        result.AvatarUrl.Should().Be("https://example.com/avatar.png");

        var stored = await db.Users.FirstAsync(u => u.Id == user.Id);
        stored.UpdatedAt.Should().BeAfter(user.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_Throws_KeyNotFoundException_WhenUserMissing()
    {
        await using var db = CreateContext();
        var service = new UserService(db);
        var request = new UpsertUserRequest { DisplayName = "Someone" };

        var act = async () => await service.UpdateAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("   ")]
    public async Task UpdateAsync_Throws_ArgumentException_WhenDisplayNameTooShort(string displayName)
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var request = new UpsertUserRequest { DisplayName = displayName };

        var act = async () => await service.UpdateAsync(user.Id, request);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_Throws_ArgumentException_WhenDisplayNameTooLong()
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var request = new UpsertUserRequest { DisplayName = new string('a', 101) };

        var act = async () => await service.UpdateAsync(user.Id, request);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_Throws_ArgumentException_WhenDateOfBirthInFuture()
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var request = new UpsertUserRequest
        {
            DisplayName = "Valid Name",
            DateOfBirth = DateTime.UtcNow.Date.AddDays(1)
        };

        var act = async () => await service.UpdateAsync(user.Id, request);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://example.com/file.png")]
    public async Task UpdateAsync_Throws_ArgumentException_WhenAvatarUrlInvalid(string avatarUrl)
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var request = new UpsertUserRequest
        {
            DisplayName = "Valid Name",
            AvatarUrl = avatarUrl
        };

        var act = async () => await service.UpdateAsync(user.Id, request);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_Succeeds_WhenDateOfBirthAndAvatarUrlAreNull()
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var request = new UpsertUserRequest { DisplayName = "Valid Name" };

        var result = await service.UpdateAsync(user.Id, request);

        result.DateOfBirth.Should().BeNull();
        result.AvatarUrl.Should().BeNull();
    }

    // ---------- CreateByAdminAsync ----------

    [Fact]
    public async Task CreateByAdminAsync_CreatesUser_WithNullAuth0Id_AndNoRoles()
    {
        await using var db = CreateContext();
        var service = new UserService(db);
        var request = new UpsertUserRequest { DisplayName = "Admin Created" };

        var dto = await service.CreateByAdminAsync(request);

        dto.DisplayName.Should().Be("Admin Created");
        dto.Roles.Should().BeEmpty();

        var stored = await db.Users.FirstAsync(u => u.Id == dto.Id);
        stored.Auth0Id.Should().BeNull();
    }

    [Fact]
    public async Task CreateByAdminAsync_Throws_ArgumentException_ForInvalidRequest()
    {
        await using var db = CreateContext();
        var service = new UserService(db);
        var request = new UpsertUserRequest { DisplayName = "" };

        var act = async () => await service.CreateByAdminAsync(request);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ---------- RemoveAllRolesAsync ----------

    [Fact]
    public async Task RemoveAllRolesAsync_RemovesMultipleRoles()
    {
        await using var db = CreateContext();
        var user = NewUser();
        user.Roles.Add(new UserRoleEntity { Id = Guid.NewGuid(), UserId = user.Id, Name = "User", AssignedAt = DateTime.UtcNow });
        user.Roles.Add(new UserRoleEntity { Id = Guid.NewGuid(), UserId = user.Id, Name = "Admin", AssignedAt = DateTime.UtcNow });
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.RemoveAllRolesAsync(user.Id);

        result.Roles.Should().BeEmpty();
        (await db.UserRoles.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task RemoveAllRolesAsync_IsNoOp_WhenAlreadyZeroRoles()
    {
        await using var db = CreateContext();
        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.RemoveAllRolesAsync(user.Id);

        result.Roles.Should().BeEmpty();
        result.DisplayName.Should().Be(user.DisplayName);
    }

    [Fact]
    public async Task RemoveAllRolesAsync_Throws_KeyNotFoundException_WhenUserMissing()
    {
        await using var db = CreateContext();
        var service = new UserService(db);

        var act = async () => await service.RemoveAllRolesAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
