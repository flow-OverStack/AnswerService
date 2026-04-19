using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class PostAnswerHandlerTests
{
    private readonly PostAnswerHandler _postAnswerHandler = new(
        MockRepositoriesGetters.GetMockAnswerRepository().Object,
        MockEntityProvidersGetters.GetMockUserProvider().Object,
        MockEntityProvidersGetters.GetMockQuestionProvider().Object,
        MapperConfiguration.GetMapperConfiguration());

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success()
    {
        //Arrange
        var command = new PostAnswerCommand("Test Answer", 4, 1);

        //Act
        var result = await _postAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_UserNotFound()
    {
        //Arrange
        var command = new PostAnswerCommand("Test Answer", 0, 1);

        //Act
        var result = await _postAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var command = new PostAnswerCommand("Test Answer", 4, 0);

        //Act
        var result = await _postAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_AnswerAlreadyExists()
    {
        //Arrange
        var command = new PostAnswerCommand("Test Answer", 1, 1);

        //Act
        var result = await _postAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}