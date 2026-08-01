using Microsoft.EntityFrameworkCore;
using Vesta.Domain.Entities;

namespace Vesta.Application.Interfaces;

/// <summary>
/// Represents the database context for the Vesta application, providing access to the various entity sets and methods
/// for saving changes to the database.
/// </summary>
public interface IVestaDbContext
{
    /// <summary>
    /// Gets the <see cref="DbSet{UserEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<UserEntity> Users { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{UserRoleEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<UserRoleEntity> UserRoles { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{FamilyEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<FamilyEntity> Families { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{FamilyMemberEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<FamilyMemberEntity> FamilyMembers { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{InviteCodeEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<InviteCodeEntity> InviteCodes { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{QuestionEntity}"/> for the specified entity type. 
    /// </summary>
    DbSet<QuestionEntity> Questions { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{DailyQuestionEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<DailyQuestionEntity> DailyQuestions { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{AnswerEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<AnswerEntity> Answers { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{CommentEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<CommentEntity> Comments { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{NotificationEntity}"/> for the specified entity type.
    /// </summary>
    DbSet<NotificationEntity> Notifications { get; }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}