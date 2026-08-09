using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vesta.Application.DTOs;
using Vesta.Domain.Entities;

namespace Vesta.Application.Mappers
{
    public static class UserRoleMapper
    {
        public static IList<RoleDto> ToDto(this ICollection<UserRoleEntity>? userRoles)
        {
            return userRoles?.Select(ToDto).ToList() ?? [];
        }

        public static RoleDto ToDto(this UserRoleEntity userRole)
        {
            return new RoleDto
            {
                Id = userRole.Role.Id,
                Name = userRole.Role.Name
            };
        }
    }
}