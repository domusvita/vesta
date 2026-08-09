using Vesta.Application.DTOs;
using Vesta.Domain.Entities;

namespace Vesta.Application.Mappers;

public static class UserMapper
{
    public static IList<UserDto> ToDto(this IList<UserEntity>? users)
    {
        return users?.Select(ToDto).ToList() ?? [];
    }

    public static UserDto ToDto(this UserEntity user)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl ?? string.Empty,
            Roles = user.Roles.ToDto(),
            CreatedAt = user.CreatedAt
        };
    }
}