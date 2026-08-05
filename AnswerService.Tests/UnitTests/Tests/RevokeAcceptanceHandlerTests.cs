using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.UnitTests.Fixtures;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class RevokeAcceptanceHandlerTests
{
    private readonly RevokeAcceptanceHandler _revokeAcceptanceHandler = new(
        RepositoryMocks.GetMockUnitOfWork().Object,
        EntityProviderMocks.GetMockUserProvider().Object,
        EntityProviderMocks.GetMockQuestionProvider().Object,
        BaseEventProducerFixture.GetBaseEventProducerConfiguration(),
        MapperFixture.GetMapperConfiguration());

    [Fact]
    public async Task Handle_QuestionAuthorRevokesAcceptedAnswer_ReturnsSuccess()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(2, 3);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentUserId_ReturnsUserNotFound()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(2, 0);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentAnswerId_ReturnsAnswerNotFound()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(0, 3);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_AnswerWithNonExistentQuestion_ReturnsQuestionNotFound()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(5, 3);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NonQuestionAuthorUserId_ReturnsForbidden()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(2, 1);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_UnacceptedAnswer_ReturnsAnswerNotAccepted()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(1, 3);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotAccepted, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}