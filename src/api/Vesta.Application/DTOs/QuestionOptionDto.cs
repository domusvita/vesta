namespace Vesta.Application.DTOs;

public record QuestionOptionDto
{
    public Guid Id { get; init; }

    public string Label { get; init; } = string.Empty;

    public bool IsOther { get; init; }

    public int SortOrder { get; init; }
}
