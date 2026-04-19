using AnswerService.Domain.Entities;
using AnswerService.Domain.Enums;
using AnswerService.Domain.Interfaces.Database;
using AnswerService.Domain.Interfaces.Repository;
using MockQueryable.Moq;
using Moq;

namespace AnswerService.Tests.Configurations;

internal static class MockRepositoriesGetters
{
    private static IMock<ITransaction> GetMockTransaction()
    {
        return new Mock<ITransaction>();
    }

    public static IMock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Answers).Returns(GetMockAnswerRepository().Object);
        mockUnitOfWork.Setup(x => x.Votes).Returns(GetMockVoteRepository().Object);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMockTransaction().Object);

        return mockUnitOfWork;
    }

    public static IMock<IBaseRepository<Answer>> GetMockAnswerRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Answer>>();
        var answers = GetAnswers().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(answers.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Answer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Answer answer, CancellationToken _) => answer);
        mockRepository.Setup(x => x.Remove(It.IsAny<Answer>())).Returns((Answer answer) => answer);
        mockRepository.Setup(x => x.Update(It.IsAny<Answer>())).Returns((Answer answer) => answer);

        return mockRepository;
    }

    public static IMock<IBaseRepository<Vote>> GetMockVoteRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Vote>>();
        var votes = GetVotes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(votes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Vote>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vote vote, CancellationToken _) => vote);
        mockRepository.Setup(x => x.Remove(It.IsAny<Vote>())).Returns((Vote vote) => vote);
        mockRepository.Setup(x => x.Update(It.IsAny<Vote>())).Returns((Vote vote) => vote);

        return mockRepository;
    }

    public static IMock<IBaseRepository<VoteType>> GetMockVoteTypeRepository()
    {
        var mockRepository = new Mock<IBaseRepository<VoteType>>();
        var voteTypes = GetVoteTypes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(voteTypes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<VoteType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VoteType voteType, CancellationToken _) => voteType);
        mockRepository.Setup(x => x.Remove(It.IsAny<VoteType>())).Returns((VoteType voteType) => voteType);
        mockRepository.Setup(x => x.Update(It.IsAny<VoteType>())).Returns((VoteType voteType) => voteType);

        return mockRepository;
    }

    public static IMock<IBaseRepository<T>> GetEmptyMockRepository<T>() where T : class
    {
        var mockRepository = new Mock<IBaseRepository<T>>();
        var entities = Array.Empty<T>().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(entities.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((T entity, CancellationToken _) => entity);
        mockRepository.Setup(x => x.Update(It.IsAny<T>())).Returns((T entity) => entity);
        mockRepository.Setup(x => x.Remove(It.IsAny<T>())).Returns((T entity) => entity);

        return mockRepository;
    }

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
                Votes = GetVotes().Where(x => x.AnswerId == 1).ToList(),
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
                Votes = GetVotes().Where(x => x.AnswerId == 2).ToList(),
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
                Votes = GetVotes().Where(x => x.AnswerId == 3).ToList(),
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
                Votes = GetVotes().Where(x => x.AnswerId == 4).ToList(),
                IsAccepted = false
            },
            new()
            {
                Id = 5,
                UserId = 4,
                QuestionId = 0,
                Body = "Answer 5", // Author: User 0 (non-existent)
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Votes = GetVotes().Where(x => x.AnswerId == 5).ToList(),
                IsAccepted = false
            }
        }.AsQueryable();
    }

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

    public static IQueryable<VoteType> GetVoteTypes()
    {
        return new[]
        {
            GetVoteTypeUpvote(),
            GetVoteTypeDownvote()
        }.AsQueryable();
    }

    private static Vote GetUpvote(long userId, long answerId)
    {
        return new Vote
        {
            UserId = userId,
            AnswerId = answerId,
            VoteTypeId = GetVoteTypeUpvote().Id,
            VoteType = GetVoteTypeUpvote()
        };
    }

    private static Vote GetDownvote(long userId, long answerId)
    {
        return new Vote
        {
            UserId = userId,
            AnswerId = answerId,
            VoteTypeId = GetVoteTypeDownvote().Id,
            VoteType = GetVoteTypeDownvote()
        };
    }

    private static VoteType GetVoteTypeUpvote()
    {
        return new VoteType
        {
            Id = 1,
            ReputationChange = 1,
            MinReputationToVote = 15,
            Name = nameof(VoteTypes.Upvote)
        };
    }

    private static VoteType GetVoteTypeDownvote()
    {
        return new VoteType
        {
            Id = 2,
            ReputationChange = -1,
            MinReputationToVote = 125,
            Name = nameof(VoteTypes.Downvote)
        };
    }
}