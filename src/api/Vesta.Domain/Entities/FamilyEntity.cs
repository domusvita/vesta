namespace Vesta.Domain.Entities;

public class FamilyEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UserEntity Creator { get; set; } = null!;

    public ICollection<FamilyMemberEntity> Members { get; set; } = [];

    public ICollection<DailyQuestionEntity> DailyQuestions { get; set; } = [];

    public ICollection<InviteCodeEntity> InviteCodes { get; set; } = [];
}