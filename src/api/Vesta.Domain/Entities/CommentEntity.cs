namespace Vesta.Domain.Entities;

public class CommentEntity
{
    public Guid Id { get; set; }

    public Guid DailyQuestionId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DailyQuestionEntity DailyQuestion { get; set; } = null!;

    public UserEntity User { get; set; } = null!;
}