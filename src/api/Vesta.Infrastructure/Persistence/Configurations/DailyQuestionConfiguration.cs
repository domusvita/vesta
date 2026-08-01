using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vesta.Domain.Entities;

namespace Vesta.Infrastructure.Persistence.Configurations;

public class DailyQuestionConfiguration : IEntityTypeConfiguration<DailyQuestionEntity>
{
    public void Configure(EntityTypeBuilder<DailyQuestionEntity> builder)
    {
        builder.HasKey(dq => dq.Id);

        builder.HasIndex(dq => new { dq.FamilyId, dq.AssignedDate }).IsUnique();

        builder.HasOne(dq => dq.Family)
            .WithMany(f => f.DailyQuestions)
            .HasForeignKey(dq => dq.FamilyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dq => dq.Question)
            .WithMany(q => q.DailyQuestions)
            .HasForeignKey(dq => dq.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}