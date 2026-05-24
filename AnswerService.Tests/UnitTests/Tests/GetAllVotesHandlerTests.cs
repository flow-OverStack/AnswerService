using AnswerService.Application.Handlers.Get.Vote;
using AnswerService.Application.Queries.Vote;
using AnswerService.Tests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class GetAllVotesHandlerTests
{
    private readonly GetAllVotesHandler _handler = new(
        MockRepositoriesGetters.GetMockVoteRepository().Object);

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success()
    {
        //Arrange
        var query = new GetAllVotesQuery();

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(6, result.Data.Count());
    }
}