using Microsoft.EntityFrameworkCore;
using Vesta.Application.DTOs;
using Vesta.Application.Interfaces;
using Vesta.Application.Mappers;
using Vesta.Domain.Entities;

namespace Vesta.Application.Services;

/// <summary>
/// Represents a service for managing question-related operations.
/// </summary>
/// <param name="vestaDbContext">The database context for accessing question data.</param>
public class QuestionService(IVestaDbContext vestaDbContext) : IQuestionService
{
    // <inheritdoc />
    public async Task<IList<QuestionDto>> GetAllAsync()
    {
        var questions = await vestaDbContext.Questions
            .Where(q => q.IsActive)
            .Include(q => q.Options)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        return questions.Select(q => q.ToDto()).ToList();
    }

    // <inheritdoc />
    public async Task<QuestionDto> CreateAsync(CreateQuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new ArgumentException("Content is required.");
        }

        if (request.Options == null || request.Options.Count == 0)
        {
            throw new ArgumentException("At least one option is required.");
        }

        var questionEntity = new QuestionEntity
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            QuestionType = request.QuestionType,
            Category = request.Category,
            MinAge = request.MinAge,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Options = request.Options.Select(o => new QuestionOptionEntity
            {
                Id = Guid.NewGuid(),
                Label = o.Label,
                IsOther = o.IsOther,
                SortOrder = o.SortOrder
            }).ToList()
        };

        vestaDbContext.Questions.Add(questionEntity);
        await vestaDbContext.SaveChangesAsync();

        return questionEntity.ToDto();
    }

    // <inheritdoc />
    public async Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new ArgumentException("Content is required.");
        }

        if (request.Options == null || request.Options.Count == 0)
        {
            throw new ArgumentException("At least one option is required.");
        }

        var questionEntity = await vestaDbContext.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questionEntity is null)
        {
            throw new KeyNotFoundException($"Question with ID {id} not found.");
        }

        questionEntity.Content = request.Content;
        questionEntity.QuestionType = request.QuestionType;
        questionEntity.Category = request.Category;
        questionEntity.MinAge = request.MinAge;

        // Update or add options
        var existingOptionIds = questionEntity.Options.Select(o => o.Id).ToHashSet();
        var requestOptionIds = request.Options.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToHashSet();

        // Remove options not in the request
        var optionsToRemove = questionEntity.Options
            .Where(o => !requestOptionIds.Contains(o.Id))
            .ToList();

        foreach (var option in optionsToRemove)
        {
            questionEntity.Options.Remove(option);
        }

        // Update existing or add new options
        foreach (var requestOption in request.Options)
        {
            if (requestOption.Id.HasValue && existingOptionIds.Contains(requestOption.Id.Value))
            {
                var existingOption = questionEntity.Options.First(o => o.Id == requestOption.Id.Value);
                existingOption.Label = requestOption.Label;
                existingOption.IsOther = requestOption.IsOther;
                existingOption.SortOrder = requestOption.SortOrder;
            }
            else
            {
                questionEntity.Options.Add(new QuestionOptionEntity
                {
                    Id = Guid.NewGuid(),
                    Label = requestOption.Label,
                    IsOther = requestOption.IsOther,
                    SortOrder = requestOption.SortOrder
                });
            }
        }

        await vestaDbContext.SaveChangesAsync();

        return questionEntity.ToDto();
    }

    // <inheritdoc />
    public async Task<QuestionDto> DeactivateAsync(Guid id)
    {
        var questionEntity = await vestaDbContext.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (questionEntity is null)
        {
            throw new KeyNotFoundException($"Question with ID {id} not found.");
        }

        questionEntity.IsActive = false;

        await vestaDbContext.SaveChangesAsync();

        return questionEntity.ToDto();
    }
}
