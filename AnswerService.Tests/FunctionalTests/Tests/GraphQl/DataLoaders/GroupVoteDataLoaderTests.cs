using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupVoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingAnswerId_ReturnsGroupedVotes()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteDataLoader>();
        const long answerId = 2;

        //Act
        var result = await dataLoader.LoadRequiredAsync(answerId);

        //Assert
        Assert.Equal(2, result.Length); // Answer with id 2 has 2 votes
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentAnswerId_ReturnsEmptyCollection()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteDataLoader>();
        const long answerId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(answerId);

        //Assert
        Assert.Empty(result);
    }
}