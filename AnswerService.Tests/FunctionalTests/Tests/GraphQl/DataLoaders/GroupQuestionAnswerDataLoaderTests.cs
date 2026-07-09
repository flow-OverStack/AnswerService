using AnswerService.GraphQl.DataLoaders;
using AnswerService.Tests.FunctionalTests.Base;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AnswerService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupQuestionAnswerDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ExistingQuestionId_ReturnsGroupedAnswers()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupQuestionAnswerDataLoader>();
        const long questionId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(questionId);

        //Assert
        Assert.Equal(2, result.Length); // Question 1 has answers 1 and 2
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_NonExistentQuestionId_ReturnsEmptyCollection()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupQuestionAnswerDataLoader>();
        const long questionId = 999;

        //Act
        var result = await dataLoader.LoadRequiredAsync(questionId);

        //Assert
        Assert.Empty(result);
    }
}