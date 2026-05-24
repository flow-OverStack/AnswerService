using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupUserVoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserVoteDataLoader>();
        const long userId = 3;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Equal(2, result.Length); // User 3 has votes on answers 1 and 2
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_NoVotes()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserVoteDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Empty(result);
    }
}