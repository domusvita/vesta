namespace Vesta.Domain.Entities;

public class QuestionEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public string? Category { get; set; }

    public int? MinAge { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<DailyQuestionEntity> DailyQuestions { get; set; } = [];
}