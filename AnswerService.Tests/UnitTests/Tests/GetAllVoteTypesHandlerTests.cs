using AnswerService.Application.Handlers.Get.VoteType;
using AnswerService.Application.Queries.VoteType;
using AnswerService.Tests.Configurations;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class GetAllVoteTypesHandlerTests
{
    private readonly GetAllVoteTypesHandler _handler = new(
        MockRepositoriesGetters.GetMockVoteTypeRepository().Object);

    [Fact]
    public async Task Handle_VoteTypesExist_ReturnsVoteTypes()
    {
        //Arrange
        var query = new GetAllVoteTypesQuery();

        //Act
        var result = await _handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }
}