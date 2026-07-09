using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupVoteTypeVoteDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingVoteTypeId_ReturnsGroupedVotes()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteTypeVoteDataLoader>();
        const long voteTypeId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(voteTypeId);

        //Assert
        Assert.Equal(4, result.Length);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentVoteTypeId_ReturnsEmptyCollection()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupVoteTypeVoteDataLoader>();
        const long voteTypeId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(voteTypeId);

        //Assert
        Assert.Empty(result);
    }
}