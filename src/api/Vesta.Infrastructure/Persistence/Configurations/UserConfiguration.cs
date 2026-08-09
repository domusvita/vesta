using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the entity type mapping for <see cref="UserEntity"/> in the Entity Framework Core
/// DbContext.
/// </summary>
/// <remarks>
/// This configuration establishes the database schema for users, including primary key definition,
/// required properties, maximum lengths, column types, and unique constraints. It ensures data
/// integrity and proper indexing for optimal query performance.
/// </remarks>
public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    /// <summary>
    /// Configures the entity type mapping for <see cref="UserEntity"/>.
    /// </summary>
    /// <remarks>
    /// Sets up the database schema including the primary key on <see cref="UserEntity.Id"/>, defines
    /// required properties with maximum length constraints, creates a unique index on
    /// <see cref="UserEntity.Auth0Id"/>, and configures timestamp columns with PostgreSQL-specific
    /// column types.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="EntityTypeBuilder{UserEntity}"/> used to configure the entity mapping.
    /// </param>
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Auth0Id).HasMaxLength(128);

        builder.HasIndex(u => u.Auth0Id).IsUnique();

        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);

        builder.Property(u => u.CreatedAt).HasColumnType("timestamptz");

        builder.Property(u => u.UpdatedAt).HasColumnType("timestamptz");
    }
}