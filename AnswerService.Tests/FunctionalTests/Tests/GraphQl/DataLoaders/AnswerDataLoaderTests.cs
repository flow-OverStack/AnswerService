using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class AnswerDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<AnswerDataLoader>();
        const long answerId = 1;

        //Act
        var result = await dataLoader.LoadAsync(answerId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(answerId, result.Id);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Null()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<AnswerDataLoader>();
        const long answerId = 0;

        //Act
        var result = await dataLoader.LoadAsync(answerId);

        //Assert
        Assert.Null(result);
    }
}