using FluentAssertions;
using Vesta.Application.Mappers;
using Vesta.Domain.Entities;
using Xunit;

namespace Vesta.UnitTests.Application;

public class UserMapperTests
{
    [Fact]
    public void ToDto_MapsAllFields_WithMultipleRolesAndValues()
    {
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var dob = new DateTime(1990, 5, 15);

        var entity = new UserEntity
        {
            Id = id,
            Auth0Id = "auth0|abc",
            DisplayName = "Jane Doe",
            DateOfBirth = dob,
            AvatarUrl = "https://example.com/avatar.png",
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
            Roles =
            [
                new UserRoleEntity { Id = Guid.NewGuid(), UserId = id, Name = "User", AssignedAt = createdAt },
                new UserRoleEntity { Id = Guid.NewGuid(), UserId = id, Name = "Admin", AssignedAt = createdAt }
            ]
        };

        var dto = entity.ToDto();

        dto.Id.Should().Be(id);
        dto.DisplayName.Should().Be("Jane Doe");
        dto.DateOfBirth.Should().Be(dob);
        dto.AvatarUrl.Should().Be("https://example.com/avatar.png");
        dto.CreatedAt.Should().Be(createdAt);
        dto.Roles.Should().BeEquivalentTo(["User", "Admin"]);
    }

    [Fact]
    public void ToDto_MapsNullAvatarUrlAndDateOfBirth_AndEmptyRoles()
    {
        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Auth0Id = null,
            DisplayName = "No Extras",
            DateOfBirth = null,
            AvatarUrl = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = []
        };

        var dto = entity.ToDto();

        dto.DateOfBirth.Should().BeNull();
        dto.AvatarUrl.Should().BeNull();
        dto.Roles.Should().BeEmpty();
    }
}
