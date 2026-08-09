namespace Vesta.Application.DTOs;

public record UpdateQuestionRequest
{
    public string Content { get; init; } = string.Empty;

    public string QuestionType { get; init; } = "MultipleChoice";

    public string? Category { get; init; }

    public int? MinAge { get; init; }

    public IList<UpdateQuestionOptionRequest> Options { get; init; } = [];
}

public record UpdateQuestionOptionRequest
{
    public Guid? Id { get; init; }

    public string Label { get; init; } = string.Empty;

    public bool IsOther { get; init; }

    public int SortOrder { get; init; }
}
