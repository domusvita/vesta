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
                    Name = "User",
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
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        return userEntities.Select(u => u.ToDto()).ToList();
    }
}