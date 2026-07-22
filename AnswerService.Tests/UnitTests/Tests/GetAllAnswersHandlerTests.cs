using AnswerService.Application.Handlers.Get.Answer;
using AnswerService.Application.Queries.Answer;
using AnswerService.Tests.Configurations;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class GetAllAnswersHandlerTests
{
    private readonly GetAllAnswersHandler _handler = new(
        MockRepositoriesGetters.GetMockAnswerRepository().Object);

    [Fact]
    public async Task Handle_NoFilter_ReturnsAllAnswers()
    {
        //Arrange
        var query = new GetAllAnswersQuery();

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(5, result.Data.Count());
    }
}