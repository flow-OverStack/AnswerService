using AnswerService.Domain.Dto.Vote;
using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class VoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadAsync_ExistingVoteKey_ReturnsVote()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<VoteDataLoader>();
        var dto = new VoteDto(3, 1);

        //Act
        var result = await dataLoader.LoadAsync(dto);

        //Assert
        Assert.NotNull(result);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadAsync_NonExistentVoteKey_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<VoteDataLoader>();
        var dto = new VoteDto(0, 0);

        //Act
        var result = await dataLoader.LoadAsync(dto);

        //Assert
        Assert.Null(result);
    }
}