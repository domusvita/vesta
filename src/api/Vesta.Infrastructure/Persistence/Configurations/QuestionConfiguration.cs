using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<QuestionEntity>
{
    public void Configure(EntityTypeBuilder<QuestionEntity> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Content).IsRequired();

        builder.Property(q => q.QuestionType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("MultipleChoice");

        builder.Property(q => q.Category)
            .HasMaxLength(100);

        builder.Property(q => q.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(q => q.DailyQuestions)
            .WithOne(dq => dq.Question)
            .HasForeignKey(dq => dq.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
