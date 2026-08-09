using Vesta.Application.DTOs;

namespace Vesta.Application.Interfaces;

/// <summary>
/// Represents a service for managing question-related operations.
/// </summary>
public interface IQuestionService
{
    /// <summary>
    /// Retrieves all active questions.
    /// </summary>
    /// <returns>A list of all active questions with their options.</returns>
    Task<IList<QuestionDto>> GetAllAsync();

    /// <summary>
    /// Creates a new question with the specified options.
    /// </summary>
    /// <param name="request">The request containing question details and options.</param>
    /// <returns>The created question DTO.</returns>
    Task<QuestionDto> CreateAsync(CreateQuestionRequest request);

    /// <summary>
    /// Updates an existing question by its ID.
    /// </summary>
    /// <param name="id">The ID of the question to update.</param>
    /// <param name="request">The request containing updated question details and options.</param>
    /// <returns>The updated question DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the question is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the request contains invalid data.</exception>
    Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionRequest request);

    /// <summary>
    /// Soft-deletes a question by setting IsActive to false.
    /// </summary>
    /// <param name="id">The ID of the question to deactivate.</param>
    /// <returns>The deactivated question DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the question is not found.</exception>
    Task<QuestionDto> DeactivateAsync(Guid id);
}
