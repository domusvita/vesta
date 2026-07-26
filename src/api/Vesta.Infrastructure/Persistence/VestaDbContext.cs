using Microsoft.EntityFrameworkCore;
using Vesta.Application.Interfaces;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence;

/// <summary>
/// Represents the database context for the Vesta application, providing access to the underlying database and managing
/// entity sets.
/// </summary>
/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
public class VestaDbContext(DbContextOptions<VestaDbContext> options) : DbContext(options), IVestaDbContext
{
    // <inheritdoc />
    public DbSet<UserEntity> Users => Set<UserEntity>();

    // <inheritdoc />
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();

    // <inheritdoc />
    public DbSet<FamilyEntity> Families => Set<FamilyEntity>();

    // <inheritdoc />
    public DbSet<FamilyMemberEntity> FamilyMembers => Set<FamilyMemberEntity>();

    // <inheritdoc />
    public DbSet<InviteCodeEntity> InviteCodes => Set<InviteCodeEntity>();

    // <inheritdoc />
    public DbSet<QuestionEntity> Questions => Set<QuestionEntity>();

    // <inheritdoc />
    public DbSet<DailyQuestionEntity> DailyQuestions => Set<DailyQuestionEntity>();

    // <inheritdoc />
    public DbSet<AnswerEntity> Answers => Set<AnswerEntity>();

    // <inheritdoc />
    public DbSet<CommentEntity> Comments => Set<CommentEntity>();

    // <inheritdoc />
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

    // <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VestaDbContext).Assembly);
    }
}