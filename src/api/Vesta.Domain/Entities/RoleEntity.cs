using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vesta.Domain.Entities;

public class RoleEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
}
