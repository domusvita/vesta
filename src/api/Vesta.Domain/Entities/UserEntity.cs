using System.Text.Json;

namespace Vesta.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; set; }

    public string? Auth0Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<FamilyMemberEntity> FamilyMemberships { get; set; } = [];

    public ICollection<UserRoleEntity> Roles { get; set; } = [];

    public ICollection<AnswerEntity> Answers { get; set; } = [];

    public ICollection<CommentEntity> Comments { get; set; } = [];

    public ICollection<NotificationEntity> Notifications { get; set; } = [];
}