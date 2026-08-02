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

    /// <summary>
    /// Updates an existing user's profile fields.
    /// </summary>
    /// <param name="id">The id of the user to update.</param>
    /// <param name="request">The fields to update.</param>
    /// <returns>A <see cref="UserDto"/> representing the updated user.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user with the given id exists.</exception>
    /// <exception cref="ArgumentException">Thrown if the request fails validation.</exception>
    Task<UserDto> UpdateAsync(Guid id, UpsertUserRequest request);

    /// <summary>
    /// Creates a new user on behalf of an admin. The created user has no Auth0 identity
    /// (<c>Auth0Id</c> is null) and is assigned no default role.
    /// </summary>
    /// <param name="request">The fields for the new user.</param>
    /// <returns>A <see cref="UserDto"/> representing the newly created user.</returns>
    /// <exception cref="ArgumentException">Thrown if the request fails validation.</exception>
    Task<UserDto> CreateByAdminAsync(UpsertUserRequest request);

    /// <summary>
    /// Removes all role assignments from a user.
    /// </summary>
    /// <param name="id">The id of the user whose roles should be removed.</param>
    /// <returns>A <see cref="UserDto"/> representing the user after roles are removed.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no user with the given id exists.</exception>
    Task<UserDto> RemoveAllRolesAsync(Guid id);
}