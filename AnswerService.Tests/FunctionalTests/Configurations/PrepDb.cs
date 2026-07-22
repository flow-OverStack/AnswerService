using AnswerService.DAL;
using AnswerService.Domain.Entities;
using AnswerService.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.Tests.FunctionalTests.Configurations;

internal static class PrepDb
{
    public static void PrepPopulation(this IServiceScope serviceScope)
    {
        var answers = AnswerMother.GetAnswers()
            .Select(x => new Answer
            {
                Id = 0,
                Body = x.Body,
                QuestionId = x.QuestionId,
                UserId = x.UserId,
                IsAccepted = x.IsAccepted
            });

        var votes = VoteMother.GetVotes().ToList();
        var voteTypes = VoteTypeMother.GetVoteTypes().ToList();

        voteTypes.ForEach(x => x.Id = 0);
        votes.ForEach(x => x.VoteType = null!);

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        dbContext.Set<Answer>().AddRange(answers);
        dbContext.Set<VoteType>().AddRange(voteTypes);
        dbContext.Set<Vote>().AddRange(votes);

        dbContext.SaveChanges();
    }
}