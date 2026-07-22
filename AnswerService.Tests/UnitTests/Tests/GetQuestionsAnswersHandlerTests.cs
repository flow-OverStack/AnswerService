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
public class GetQuestionsAnswersHandlerTests
{
    private readonly CacheGetQuestionsAnswersHandler _handler = new(
        new AnswerCacheRepository(
            new RedisCacheProvider(RedisDatabaseFixture.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsFixture.GetRedisSettingsConfiguration())),
        new GetQuestionsAnswersHandler(
            RepositoryMocks.GetMockAnswerRepository().Object)
    );

    [Fact]
    public async Task Handle_ExistingQuestionIds_ReturnsAnswers()
    {
        //Arrange
        var query = new GetQuestionsAnswersQuery([1, 2, 0]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentQuestionId_ReturnsAnswersNotFound()
    {
        //Arrange
        var query = new GetQuestionsAnswersQuery([-1]);

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}