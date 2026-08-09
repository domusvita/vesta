namespace Vesta.Domain.Entities;

public class QuestionOptionEntity
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string Label { get; set; } = string.Empty;

    public bool IsOther { get; set; } = false;

    public int SortOrder { get; set; }

    public QuestionEntity Question { get; set; } = null!;
}
