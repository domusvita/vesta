using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vesta.Application.DTOs;
using Vesta.Application.Services;
using Vesta.Domain.Entities;
using Vesta.Infrastructure.Persistence;
using Xunit;

namespace Vesta.UnitTests.Application;

public class QuestionServiceTests
{
    private static VestaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VestaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new VestaDbContext(options);
    }

    private static QuestionEntity NewQuestion(string content = "What's your favorite color?", string category = "Preferences")
    {
        return new QuestionEntity
        {
            Id = Guid.NewGuid(),
            Content = content,
            QuestionType = "MultipleChoice",
            Category = category,
            MinAge = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Options =
            [
                new() { Id = Guid.NewGuid(), Label = "Red", IsOther = false, SortOrder = 0 },
                new() { Id = Guid.NewGuid(), Label = "Blue", IsOther = false, SortOrder = 1 },
                new() { Id = Guid.NewGuid(), Label = "Green", IsOther = false, SortOrder = 2 },
                new() { Id = Guid.NewGuid(), Label = "Other", IsOther = true, SortOrder = 3 }
            ]
        };
    }

    // ---------- GetAllAsync ----------

    [Fact]
    public async Task GetAllAsync_ReturnsAllActiveQuestions_InReverseChronologicalOrder()
    {
        await using var db = CreateContext();
        var question1 = NewQuestion("First Question");
        var question2 = NewQuestion("Second Question");
        question2.CreatedAt = question1.CreatedAt.AddHours(1);
        
        db.Questions.AddRange(question1, question2);
        await db.SaveChangesAsync();

        var service = new QuestionService(db);
        var result = await service.GetAllAsync();

        result.Should().HaveCount(2);
        result[0].Content.Should().Be("Second Question");
        result[1].Content.Should().Be("First Question");
    }

    [Fact]
    public async Task GetAllAsync_ExcludesInactiveQuestions()
    {
        await using var db = CreateContext();
        var activeQuestion = NewQuestion("Active");
        var inactiveQuestion = NewQuestion("Inactive");
        inactiveQuestion.IsActive = false;
        
        db.Questions.AddRange(activeQuestion, inactiveQuestion);
        await db.SaveChangesAsync();

        var service = new QuestionService(db);
        var result = await service.GetAllAsync();

        result.Should().HaveCount(1);
        result[0].Content.Should().Be("Active");
    }

    [Fact]
    public async Task GetAllAsync_IncludesOptionsInCorrectOrder()
    {
        await using var db = CreateContext();
        var question = NewQuestion();
        db.Questions.Add(question);
        await db.SaveChangesAsync();

        var service = new QuestionService(db);
        var result = await service.GetAllAsync();

        result[0].Options.Should().HaveCount(4);
        result[0].Options[0].Label.Should().Be("Red");
        result[0].Options[3].Label.Should().Be("Other");
    }

    // ---------- CreateAsync ----------

    [Fact]
    public async Task CreateAsync_CreatesQuestionWithOptions_AndSetsIsActiveTrue()
    {
        await using var db = CreateContext();
        var request = new CreateQuestionRequest
        {
            Content = "New Question",
            QuestionType = "MultipleChoice",
            Category = "Test",
            MinAge = 10,
            Options =
            [
                new() { Label = "Option A", IsOther = false, SortOrder = 0 },
                new() { Label = "Option B", IsOther = false, SortOrder = 1 },
                new() { Label = "Other", IsOther = true, SortOrder = 2 }
            ]
        };

        var service = new QuestionService(db);
        var result = await service.CreateAsync(request);

        result.Content.Should().Be("New Question");
        result.IsActive.Should().BeTrue();
        result.Options.Should().HaveCount(3);
        result.Options[0].Label.Should().Be("Option A");

        var saved = await db.Questions.FirstAsync(q => q.Id == result.Id);
        saved.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenContentIsEmpty()
    {
        await using var db = CreateContext();
        var request = new CreateQuestionRequest { Content = "" };

        var service = new QuestionService(db);
        var act = () => service.CreateAsync(request);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Content*");
    }

    [Fact]
    public async Task CreateAsync_ThrowsArgumentException_WhenNoOptionsProvided()
    {
        await using var db = CreateContext();
        var request = new CreateQuestionRequest { Content = "Question", Options = [] };

        var service = new QuestionService(db);
        var act = () => service.CreateAsync(request);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*option*");
    }

    // ---------- UpdateAsync ----------

    [Fact]
    public async Task UpdateAsync_UpdatesQuestionContent_AndOptions()
    {
        await using var db = CreateContext();
        var question = NewQuestion();
        db.Questions.Add(question);
        await db.SaveChangesAsync();

        var request = new UpdateQuestionRequest
        {
            Content = "Updated Question",
            QuestionType = "MultipleChoice",
            Category = "Updated",
            MinAge = 15,
            Options =
            [
                new() { Label = "New Option A", IsOther = false, SortOrder = 0 },
                new() { Label = "New Option B", IsOther = false, SortOrder = 1 }
            ]
        };

        var service = new QuestionService(db);
        var result = await service.UpdateAsync(question.Id, request);

        result.Content.Should().Be("Updated Question");
        result.Category.Should().Be("Updated");
        result.MinAge.Should().Be(15);
        result.Options.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsKeyNotFoundException_WhenQuestionNotFound()
    {
        await using var db = CreateContext();
        var request = new UpdateQuestionRequest { Content = "Updated" };

        var service = new QuestionService(db);
        var act = () => service.UpdateAsync(Guid.NewGuid(), request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ThrowsArgumentException_WhenContentIsEmpty()
    {
        await using var db = CreateContext();
        var question = NewQuestion();
        db.Questions.Add(question);
        await db.SaveChangesAsync();

        var request = new UpdateQuestionRequest { Content = "" };
        var service = new QuestionService(db);
        var act = () => service.UpdateAsync(question.Id, request);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Content*");
    }

    // ---------- DeactivateAsync ----------

    [Fact]
    public async Task DeactivateAsync_SetsIsActiveToFalse()
    {
        await using var db = CreateContext();
        var question = NewQuestion();
        db.Questions.Add(question);
        await db.SaveChangesAsync();

        var service = new QuestionService(db);
        var result = await service.DeactivateAsync(question.Id);

        result.IsActive.Should().BeFalse();

        var saved = await db.Questions.FirstAsync(q => q.Id == question.Id);
        saved.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeactivateAsync_ThrowsKeyNotFoundException_WhenQuestionNotFound()
    {
        await using var db = CreateContext();
        var service = new QuestionService(db);
        var act = () => service.DeactivateAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
