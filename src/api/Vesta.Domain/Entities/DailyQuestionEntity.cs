namespace Vesta.Domain.Entities;

public class DailyQuestionEntity
{
    public Guid Id { get; set; }

    public Guid FamilyId { get; set; }

    public Guid QuestionId { get; set; }

    public DateOnly AssignedDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public FamilyEntity Family { get; set; } = null!;

    public QuestionEntity Question { get; set; } = null!;

    public ICollection<AnswerEntity> Answers { get; set; } = [];

    public ICollection<CommentEntity> Comments { get; set; } = [];
}