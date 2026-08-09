using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vesta.Application.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}