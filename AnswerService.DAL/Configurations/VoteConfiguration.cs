using AnswerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnswerService.DAL.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.AnswerId).IsRequired();
        builder.HasQueryFilter(x => x.Answer.Enabled);

        builder.HasKey(x => new { x.AnswerId, x.UserId });
    }
}