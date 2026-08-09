using Microsoft.EntityFrameworkCore;
using Vesta.Application.DTOs;
using Vesta.Application.Interfaces;
using Vesta.Application.Mappers;
using Vesta.Domain.Entities;

namespace Vesta.Application.Services;

/// <summary>
/// Represents a service for managing user-related operations.
/// </summary>
/// <param name="vestaDbContext">The database context for accessing user data.</param>
public class UserService(IVestaDbContext vestaDbContext) : IUserService
{
    // <inheritdoc />
    public async Task<UserDto?> GetByAuthIdAsync(string? authId)
    {
        var userEntity = await vestaDbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Auth0Id == authId);

        return userEntity?.ToDto();
    }

    // <inheritdoc />
    public async Task<UserDto> CreateAsync(string auth0Id, string displayName)
    {
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Auth0Id = auth0Id,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles =
            [
                new UserRoleEntity
                {
                    Id = Guid.NewGuid(),
                    RoleId = Guid.NewGuid(), // Replace with the actual RoleId for the "User" role
                    AssignedAt = DateTime.UtcNow
                }
            ]
        };

        vestaDbContext.Users.Add(userEntity);
        await vestaDbContext.SaveChangesAsync();

        return userEntity.ToDto();
    }

    // <inheritdoc />
    public async Task<IList<UserDto>> GetAllAsync()
    {
        var userEntities = await vestaDbContext.Users
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        return userEntities.Select(u => u.ToDto()).ToList();
    }

    // <inheritdoc />
    public async Task<UserDto> UpdateAsync(Guid id, UpsertUserRequest request)
    {
        ValidateUpsertRequest(request);

        var userEntity = await vestaDbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (userEntity is null)
        {
            throw new KeyNotFoundException($"User with id '{id}' was not found.");
        }

        userEntity.DisplayName = request.DisplayName.Trim();
        userEntity.DateOfBirth = request.DateOfBirth;
        userEntity.AvatarUrl = request.AvatarUrl;
        userEntity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        await vestaDbContext.SaveChangesAsync();

        return userEntity.ToDto();
    }

    // <inheritdoc />
    public async Task<UserDto> CreateByAdminAsync(UpsertUserRequest request)
    {
        ValidateUpsertRequest(request);

        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Auth0Id = null,
            DisplayName = request.DisplayName.Trim(),
            DateOfBirth = request.DateOfBirth,
            AvatarUrl = request.AvatarUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        vestaDbContext.Users.Add(userEntity);
        await vestaDbContext.SaveChangesAsync();

        return userEntity.ToDto();
    }

    // <inheritdoc />
    public async Task<UserDto> RemoveAllRolesAsync(Guid id)
    {
        var userEntity = await vestaDbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (userEntity is null)
        {
            throw new KeyNotFoundException($"User with id '{id}' was not found.");
        }

        if (userEntity.Roles.Count > 0)
        {
            foreach (var role in userEntity.Roles.ToList())
            {
                vestaDbContext.UserRoles.Remove(role);
            }

            userEntity.Roles.Clear();
            await vestaDbContext.SaveChangesAsync();
        }

        return userEntity.ToDto();
    }

    private static void ValidateUpsertRequest(UpsertUserRequest request)
    {
        var displayName = request.DisplayName?.Trim() ?? string.Empty;
        if (displayName.Length is < 2 or > 100)
        {
            throw new ArgumentException("DisplayName is required and must be between 2 and 100 characters.");
        }

        if (request.DateOfBirth.HasValue && request.DateOfBirth.Value.Date > DateTime.UtcNow.Date)
        {
            throw new ArgumentException("DateOfBirth cannot be in the future.");
        }

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            var isValidUri = Uri.TryCreate(request.AvatarUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

            if (!isValidUri)
            {
                throw new ArgumentException("AvatarUrl must be a well-formed absolute http or https URL.");
            }
        }
    }
}
