using Vesta.Application.DTOs;
using Vesta.Domain.Entities;

namespace Vesta.Application.Mappers;

/// <summary>
/// Provides extension methods for mapping RoleEntity objects to RoleDto objects.
/// </summary>
public static class RoleMapper
{
    /// <summary>
    /// Converts a list of RoleEntity objects to a list of RoleDto objects.
    /// </summary>
    /// <param name="roles">The list of RoleEntity objects to convert.</param>
    /// <returns>The corresponding list of RoleDto objects.</returns>
    public static IList<RoleDto> ToDto(this IList<RoleEntity>? roles)
    {
        return roles?.Select(ToDto).ToList() ?? [];
    }

    /// <summary>
    /// Converts a RoleEntity to a RoleDto.
    /// </summary>
    /// <param name="role">The RoleEntity to convert.</param>
    /// <returns>The corresponding RoleDto.</returns>
    public static RoleDto ToDto(this RoleEntity role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}
