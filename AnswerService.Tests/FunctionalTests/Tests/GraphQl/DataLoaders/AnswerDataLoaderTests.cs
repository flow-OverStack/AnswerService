using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class AnswerDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task Load_ExistingAnswerId_ReturnsAnswer()
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

    [Fact]
    public async Task Load_NonExistentAnswerId_ReturnsNull()
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