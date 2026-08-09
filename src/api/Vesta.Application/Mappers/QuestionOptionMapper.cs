using Vesta.Application.DTOs;
using Vesta.Domain.Entities;

namespace Vesta.Application.Mappers;

public static class QuestionOptionMapper
{
    public static QuestionOptionDto ToDto(this QuestionOptionEntity entity)
    {
        return new QuestionOptionDto
        {
            Id = entity.Id,
            Label = entity.Label,
            IsOther = entity.IsOther,
            SortOrder = entity.SortOrder
        };
    }
}
