using Vesta.Application.DTOs;
using Vesta.Domain.Entities;

namespace Vesta.Application.Mappers;

public static class QuestionMapper
{
    public static QuestionDto ToDto(this QuestionEntity entity)
    {
        return new QuestionDto
        {
            Id = entity.Id,
            Content = entity.Content,
            QuestionType = entity.QuestionType,
            Category = entity.Category,
            MinAge = entity.MinAge,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            Options = entity.Options.OrderBy(o => o.SortOrder).Select(o => o.ToDto()).ToList()
        };
    }
}
