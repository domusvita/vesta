using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMemberEntity>
{
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