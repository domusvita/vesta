namespace Vesta.Domain.Entities;

public class FamilyMemberEntity
{
    public Guid Id { get; set; }

    public Guid FamilyId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    public FamilyEntity Family { get; set; } = null!;

    public UserEntity User { get; set; } = null!;
}