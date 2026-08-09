using FluentAssertions;
using Vesta.Domain.Entities;
using Xunit;

namespace Vesta.UnitTests.Domain;

public class UserEntityTests
{
    [Fact]
    public void NewUserEntity_HasEmptyCollectionDefaults_AndEmptyDisplayName()
    {
        var entity = new UserEntity();

        entity.DisplayName.Should().Be(string.Empty);
        entity.Auth0Id.Should().BeNull();
        entity.AvatarUrl.Should().BeNull();
        entity.DateOfBirth.Should().BeNull();
        entity.FamilyMemberships.Should().BeEmpty();
        entity.Roles.Should().BeEmpty();
        entity.Answers.Should().BeEmpty();
        entity.Comments.Should().BeEmpty();
        entity.Notifications.Should().BeEmpty();
    }

    [Fact]
    public void NewUserRoleEntity_HasDefaultName()
    {
        var role = new UserRoleEntity();

        role.Name.Should().Be(string.Empty);
    }
}
