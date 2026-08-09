namespace Vesta.Application.DTOs;

public record CreateQuestionRequest
{
    public string Content { get; init; } = string.Empty;

    public string QuestionType { get; init; } = "MultipleChoice";

    public string? Category { get; init; }

    public int? MinAge { get; init; }

    public IList<CreateQuestionOptionRequest> Options { get; init; } = [];
}

public record CreateQuestionOptionRequest
{
    public string Label { get; init; } = string.Empty;

    public bool IsOther { get; init; }

    public int SortOrder { get; init; }
}
