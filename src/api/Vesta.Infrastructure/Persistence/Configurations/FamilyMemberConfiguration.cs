using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the entity type mapping for <see cref="FamilyMemberEntity"/> in the Entity Framework Core data model.
/// </summary>
/// <remarks>
/// This configuration defines the database schema, relationships, and constraints for family member entities. It establishes
/// a unique constraint on the combination of FamilyId and UserId to prevent duplicate memberships, configures cascading
/// deletion for family relationships, and restricts deletion when referenced by users to maintain referential integrity.
/// </remarks>
public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMemberEntity>
{
    /// <summary>
    /// Configures the entity type mapping for <see cref="FamilyMemberEntity"/> using the provided
    /// <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">
    /// The Entity Framework Core entity type builder used to configure the <see cref="FamilyMemberEntity"/> mapping.
    /// </param>
    public void Configure(EntityTypeBuilder<FamilyMemberEntity> builder)
    {
        builder.HasKey(fm => fm.Id);

        builder.HasIndex(fm => new { fm.FamilyId, fm.UserId }).IsUnique();

        builder.Property(fm => fm.Role).IsRequired().HasMaxLength(10);

        builder.HasOne(fm => fm.Family)
            .WithMany(f => f.Members)
            .HasForeignKey(fm => fm.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fm => fm.User)
            .WithMany(u => u.FamilyMemberships)
            .HasForeignKey(fm => fm.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}