using AnswerService.Domain.Entities;
using AnswerService.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnswerService.DAL.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Body).IsRequired().HasMaxLength(BusinessRules.BodyMaxLength);
        builder.Property(x => x.IsAccepted).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.LastModifiedAt);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.Enabled).IsRequired().HasDefaultValue(true);
        builder.HasQueryFilter(x => x.Enabled);

        builder.HasMany(x => x.Votes)
            .WithOne(x => x.Answer)
            .HasForeignKey(x => x.AnswerId)
            .HasPrincipalKey(x => x.Id);
    }
}