namespace Vesta.Domain.Entities;

public class InviteCodeEntity
{
    public Guid Id { get; set; }

    public Guid FamilyId { get; set; }

    public Guid CreatedBy { get; set; }

    public string Code { get; set; } = string.Empty;

    public string IntendedRole { get; set; } = string.Empty;

    public int MaxUses { get; set; } = 1;

    public int Uses { get; set; } = 0;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public FamilyEntity Family { get; set; } = null!;

    public UserEntity Creator { get; set; } = null!;
}