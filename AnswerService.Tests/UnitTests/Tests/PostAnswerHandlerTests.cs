using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class PostAnswerHandlerTests
{
    private readonly PostAnswerHandler _postAnswerHandler = new(
        MockRepositoriesGetters.GetMockAnswerRepository().Object,
        MockEntityProvidersGetters.GetMockUserProvider().Object,
        MockEntityProvidersGetters.GetMockQuestionProvider().Object,
        MapperConfiguration.GetMapperConfiguration());

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        //Arrange
        var command = new PostAnswerCommand("Test Answer", 4, 1);

        //Act
        var result = await _postAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsUserNotFound()
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

    [Fact]
    public async Task Handle_QuestionDoesNotExist_ReturnsQuestionNotFound()
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

    [Fact]
    public async Task Handle_AnswerAlreadyExistsForUserAndQuestion_ReturnsAnswerAlreadyExists()
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