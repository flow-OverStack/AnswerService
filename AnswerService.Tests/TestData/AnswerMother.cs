using AnswerService.Domain.Entities;

namespace AnswerService.Tests.TestData;

internal static class AnswerMother
{
    public static IQueryable<Answer> GetAnswers()
    {
        return new Answer[]
        {
            new()
            {
                Id = 1,
                UserId = 1,
                QuestionId = 1, // Author: User 3
                Body = "Answer 1",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Votes = VoteMother.GetVotes().Where(x => x.AnswerId == 1).ToList(),
                IsAccepted = false
            },
            new()
            {
                Id = 2,
                UserId = 2,
                QuestionId = 1, // Author: User 3
                Body = "Answer 2",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Votes = VoteMother.GetVotes().Where(x => x.AnswerId == 2).ToList(),
                IsAccepted = true
            },
            new()
            {
                Id = 3,
                UserId = 1,
                QuestionId = 2, // Author: User 2
                Body = "Answer 3",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Votes = VoteMother.GetVotes().Where(x => x.AnswerId == 3).ToList(),
                IsAccepted = true
            },
            new()
            {
                Id = 4,
                UserId = 3,
                QuestionId = 3, // Author: User 1
                Body = "Answer 4",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Votes = VoteMother.GetVotes().Where(x => x.AnswerId == 4).ToList(),
                IsAccepted = false
            },
            new()
            {
                Id = 5,
                UserId = 4,
                QuestionId = 0, // Author: User 1
                Body = "Answer 5",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Votes = VoteMother.GetVotes().Where(x => x.AnswerId == 5).ToList(),
                IsAccepted = true
            }
        }.AsQueryable();
    }
}
