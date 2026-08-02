using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
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