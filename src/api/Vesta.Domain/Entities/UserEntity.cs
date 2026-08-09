using System.Text.Json;

namespace Vesta.Domain.Entities;

/// <summary>
/// Represents a user in the Vesta system, managing user profile information, authentication, and relationships to other
/// entities.
/// </summary>
/// <remarks>
/// This entity serves as the core user model, integrating authentication via Auth0 with local profile data. Users can have
/// multiple roles, participate in family memberships, and contribute through answers, comments, and receive notifications.
/// </remarks>
public class UserEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the Auth0 identifier used for external authentication and identity management.
    /// </summary>
    public string? Auth0Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the user, shown throughout the application.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's date of birth, or null if not provided.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the URL to the user's avatar image, or null if not set.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the user was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of family memberships for this user.
    /// </summary>
    public ICollection<FamilyMemberEntity> FamilyMemberships { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of roles assigned to this user.
    /// </summary>
    public ICollection<UserRoleEntity> Roles { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of answers submitted by this user.
    /// </summary>
    public ICollection<AnswerEntity> Answers { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of comments created by this user.
    /// </summary>
    public ICollection<CommentEntity> Comments { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of notifications sent to this user.
    /// </summary>
    public ICollection<NotificationEntity> Notifications { get; set; } = [];
}