using AnswerService.Domain.Entities;
using AnswerService.Domain.Interfaces.Database;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Tests.TestData;
using MockQueryable.Moq;
using Moq;

namespace AnswerService.Tests.Mocks;

internal static class RepositoryMocks
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
        var answers = AnswerMother.GetAnswers().BuildMockDbSet();

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
        var votes = VoteMother.GetVotes().BuildMockDbSet();

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
        var voteTypes = VoteTypeMother.GetVoteTypes().BuildMockDbSet();

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
}
