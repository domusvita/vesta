namespace Vesta.Application.DTOs;

public record QuestionDto
{
    public Guid Id { get; init; }

    public string Content { get; init; } = string.Empty;

    public string QuestionType { get; init; } = "MultipleChoice";

    public string? Category { get; init; }

    public int? MinAge { get; init; }

    public bool IsActive { get; init; } = true;

    public DateTime CreatedAt { get; init; }

    public IList<QuestionOptionDto> Options { get; init; } = [];
}
