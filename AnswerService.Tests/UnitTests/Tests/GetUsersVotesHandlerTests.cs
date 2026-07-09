using AnswerService.Application.Handlers.Decorators.Cache.Get.Vote;
using AnswerService.Application.Handlers.Get.Vote;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
using AnswerService.Cache.Providers;
using AnswerService.Cache.Repositories;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Microsoft.Extensions.Options;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class GetUsersVotesHandlerTests
{
    private readonly CacheGetUsersVotesHandler _handler = new(
        new VoteCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration())),
        new GetUsersVotesHandler(
            MockRepositoriesGetters.GetMockVoteRepository().Object)
    );

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExistingAndNonExistentUserIds_ReturnsSuccess()
    {
        //Arrange
        var query = new GetUsersVotesQuery([1, 3, 0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_NonExistentUserId_ReturnsVotesNotFound()
    {
        //Arrange
        var query = new GetUsersVotesQuery([0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}