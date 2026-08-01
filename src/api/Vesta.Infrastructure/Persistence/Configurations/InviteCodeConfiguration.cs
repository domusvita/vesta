using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

public class InviteCodeConfiguration : IEntityTypeConfiguration<InviteCodeEntity>
{
    public void Configure(EntityTypeBuilder<InviteCodeEntity> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Code).IsRequired().HasMaxLength(16);

        builder.HasIndex(i => i.Code).IsUnique();

        builder.Property(i => i.IntendedRole).IsRequired().HasMaxLength(10);

        builder.HasOne(i => i.Family)
            .WithMany(f => f.InviteCodes)
            .HasForeignKey(i => i.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Creator)
            .WithMany()
            .HasForeignKey(i => i.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}