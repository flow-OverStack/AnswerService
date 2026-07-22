using AnswerService.Domain.Entities;

namespace AnswerService.Tests.TestData;

internal static class VoteMother
{
    public static IQueryable<Vote> GetVotes()
    {
        return new[]
        {
            GetUpvote(3, 2),
            GetDownvote(3, 1),
            GetUpvote(4, 2),
            GetUpvote(1, 3),
            GetDownvote(1, 4),
            GetUpvote(4, 4)
        }.AsQueryable();
    }

    public static Vote GetUpvote(long userId, long answerId)
    {
        return new Vote
        {
            UserId = userId,
            AnswerId = answerId,
            VoteTypeId = VoteTypeMother.GetVoteTypeUpvote().Id,
            VoteType = VoteTypeMother.GetVoteTypeUpvote()
        };
    }

    public static Vote GetDownvote(long userId, long answerId)
    {
        return new Vote
        {
            UserId = userId,
            AnswerId = answerId,
            VoteTypeId = VoteTypeMother.GetVoteTypeDownvote().Id,
            VoteType = VoteTypeMother.GetVoteTypeDownvote()
        };
    }
}
