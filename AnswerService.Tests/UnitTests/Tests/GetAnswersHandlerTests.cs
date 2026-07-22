using AnswerService.Application.Handlers.Decorators.Cache.Get.Answer;
using AnswerService.Application.Handlers.Get.Answer;
using AnswerService.Application.Queries.Answer;
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
public class GetAnswersHandlerTests
{
    private readonly CacheGetAnswersHandler _getAnswersHandler = new(
        new AnswerCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration())),
        new GetAnswersHandler(
            RepositoryMocks.GetMockAnswerRepository().Object)
    );

    [Fact]
    public async Task Handle_ExistingAndNonExistingIds_ReturnsAnswers()
    {
        //Arrange
        var query = new GetAnswersQuery([1, 2, 0]);

        //Act
        var result = await _getAnswersHandler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_SingleNonExistentId_ReturnsAnswerNotFound()
    {
        //Arrange
        var query = new GetAnswersQuery([0]);

        //Act
        var result = await _getAnswersHandler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_MultipleNonExistentIds_ReturnsAnswersNotFound()
    {
        //Arrange
        var query = new GetAnswersQuery([0, 0]);

        //Act
        var result = await _getAnswersHandler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}