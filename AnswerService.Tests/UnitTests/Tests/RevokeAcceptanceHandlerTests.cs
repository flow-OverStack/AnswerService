using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class RevokeAcceptanceHandlerTests
{
    private readonly RevokeAcceptanceHandler _revokeAcceptanceHandler = new(
        MockRepositoriesGetters.GetMockUnitOfWork().Object,
        MockEntityProvidersGetters.GetMockUserProvider().Object,
        MockEntityProvidersGetters.GetMockQuestionProvider().Object,
        BaseEventProducerConfiguration.GetBaseEventProducerConfiguration(),
        MapperConfiguration.GetMapperConfiguration());

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success()
    {
        //Arrange
        var command = new RevokeAcceptanceCommand(2, 3);

        //Act
        var result = await _revokeAcceptanceHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_UserNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_AnswerNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_QuestionNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_OperationForbidden()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_AnswerNotAccepted()
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