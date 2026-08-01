using Vesta.Application.DTOs;

namespace Vesta.Application.Interfaces;

/// <summary>
/// Represents a service for managing user-related operations. 
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their authentication ID.
    /// </summary>
    /// <param name="authId">The authentication ID of the user.</param>
    /// <returns>A <see cref="UserDto"/> representing the user, or <c>null</c> if the user is not found.</returns>
    Task<UserDto?> GetByAuthIdAsync(string? authId);

    /// <summary>
    /// Creates a new user with the specified authentication ID and display name.
    /// </summary>
    /// <param name="auth0Id">The Auth0 authentication ID of the user.</param>
    /// <param name="displayName">The display name for the new user.</param>
    /// <returns>A <see cref="UserDto"/> representing the newly created user.</returns>
    Task<UserDto> CreateAsync(string auth0Id, string displayName);

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>A list of <see cref="UserDto"/> representing all users.</returns>
    Task<IList<UserDto>> GetAllAsync();
}