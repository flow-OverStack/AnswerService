using AnswerService.Application.Handlers.Decorators.Cache.Get.VoteType;
using AnswerService.Application.Handlers.Get.VoteType;
using AnswerService.Application.Queries.VoteType;
using AnswerService.Application.Resources;
using AnswerService.Cache.Providers;
using AnswerService.Cache.Repositories;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.UnitTests.Fixtures;
using Microsoft.Extensions.Options;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class GetVoteTypesHandlerTests
{
    private readonly CacheGetVoteTypesHandler _handler = new(
        new VoteTypeCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration())),
        new GetVoteTypesHandler(
            RepositoryMocks.GetMockVoteTypeRepository().Object)
    );

    [Fact]
    public async Task Handle_ExistingAndNonExistentVoteTypeIds_ReturnsSuccess()
    {
        //Arrange
        var query = new GetVoteTypesQuery([1, 2, 0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_SingleNonExistentVoteTypeId_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var query = new GetVoteTypesQuery([0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_MultipleNonExistentVoteTypeIds_ReturnsVoteTypesNotFound()
    {
        //Arrange
        var query = new GetVoteTypesQuery([0, 0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}