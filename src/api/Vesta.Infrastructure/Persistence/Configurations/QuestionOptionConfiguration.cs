using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOptionEntity>
{
    public void Configure(EntityTypeBuilder<QuestionOptionEntity> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Label).IsRequired();

        builder.Property(o => o.IsOther).IsRequired().HasDefaultValue(false);

        builder.Property(o => o.SortOrder).IsRequired();

        builder.HasIndex(o => new { o.QuestionId, o.SortOrder });
    }
}
