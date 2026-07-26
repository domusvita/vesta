namespace Vesta.Domain.Entities;

public class UserRoleEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime AssignedAt { get; set; }

    public UserEntity User { get; set; } = null!;
}
