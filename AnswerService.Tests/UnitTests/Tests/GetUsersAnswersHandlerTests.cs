using AnswerService.Application.Handlers.Decorators.Cache.Get.Answer;
using AnswerService.Application.Handlers.Get.Answer;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Cache.Providers;
using AnswerService.Cache.Repositories;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Microsoft.Extensions.Options;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class GetUsersAnswersHandlerTests
{
    private readonly CacheGetUsersAnswersHandler _handler = new(
        new AnswerCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration())),
        new GetUsersAnswersHandler(
            MockRepositoriesGetters.GetMockAnswerRepository().Object)
    );

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ExistingUserIds_ReturnsSuccess()
    {
        //Arrange
        var query = new GetUsersAnswersQuery([1, 2, 0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_NoMatchingUserAnswers_ReturnsAnswersNotFound()
    {
        //Arrange
        var query = new GetUsersAnswersQuery([0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}