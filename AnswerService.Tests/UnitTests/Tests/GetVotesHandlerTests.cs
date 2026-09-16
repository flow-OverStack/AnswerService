using AnswerService.Application.Handlers.Decorators.Cache.Get.Vote;
using AnswerService.Application.Handlers.Get.Vote;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
using AnswerService.Cache.Providers;
using AnswerService.Cache.Repositories;
using AnswerService.Domain.Dtos.Vote;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.Traits;
using AnswerService.Tests.UnitTests.Fixtures;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class GetVotesHandlerTests
{
    private readonly CacheGetVotesHandler _handler = new(
        new VoteCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration()),
            new Mock<ILogger>().Object),
        new GetVotesHandler(
            RepositoryMocks.GetMockVoteRepository().Object)
    );

    [Fact]
    public async Task Handle_ExistingAndNonExistentVotePairs_ReturnsSuccess()
    {
        //Arrange
        var query = new GetVotesQuery([new VoteKey(3, 2), new VoteKey(1, 3), new VoteKey(0, 0)]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_SingleNonExistentVotePair_ReturnsVoteNotFound()
    {
        //Arrange
        var query = new GetVotesQuery([new VoteKey(0, 0)]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_MultipleNonExistentVotePairs_ReturnsVotesNotFound()
    {
        //Arrange
        var query = new GetVotesQuery([new VoteKey(0, 0), new VoteKey(0, 1)]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}