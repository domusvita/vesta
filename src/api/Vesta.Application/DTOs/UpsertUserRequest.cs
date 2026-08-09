namespace Vesta.Application.DTOs;

/// <summary>
/// Shared request shape for admin-driven user creation and update operations.
/// </summary>
public class UpsertUserRequest
{
    public string DisplayName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? AvatarUrl { get; set; }
}
