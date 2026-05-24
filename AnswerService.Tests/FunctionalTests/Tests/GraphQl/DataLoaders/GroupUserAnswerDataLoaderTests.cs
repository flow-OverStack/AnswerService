using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupUserAnswerDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserAnswerDataLoader>();
        const long userId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Equal(2, result.Length); // User 1 has answers 1 and 3
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_NoAnswers()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserAnswerDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Empty(result);
    }
}