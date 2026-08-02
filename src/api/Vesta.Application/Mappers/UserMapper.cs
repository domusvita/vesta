using Vesta.Application.DTOs;
using Vesta.Domain.Entities;

namespace Vesta.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this UserEntity user)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            DateOfBirth = user.DateOfBirth,
            AvatarUrl = user.AvatarUrl,
            Roles = user.Roles.Select(r => r.Name).ToList(),
            CreatedAt = user.CreatedAt
        };
    }
}