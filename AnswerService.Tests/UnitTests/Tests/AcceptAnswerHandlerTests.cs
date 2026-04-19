using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class AcceptAnswerHandlerTests
{
    private readonly AcceptAnswerHandler _acceptAnswerHandler = new(
        MockRepositoriesGetters.GetMockUnitOfWork().Object,
        MockEntityProvidersGetters.GetMockUserProvider().Object,
        MockEntityProvidersGetters.GetMockQuestionProvider().Object,
        BaseEventProducerConfiguration.GetBaseEventProducerConfiguration(),
        MapperConfiguration.GetMapperConfiguration());

    // Methods that reach ExecuteUpdateAsync are not tested here
    // Because ExecuteUpdateAsync cannot be mocked

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_UserNotFound()
    {
        //Arrange
        var command = new AcceptAnswerCommand(3, 0);

        //Act
        var result = await _acceptAnswerHandler.Handle(command, CancellationToken.None);

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
        var command = new AcceptAnswerCommand(0, 4);

        //Act
        var result = await _acceptAnswerHandler.Handle(command, CancellationToken.None);

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
        var command = new AcceptAnswerCommand(5, 4);

        //Act
        var result = await _acceptAnswerHandler.Handle(command, CancellationToken.None);

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
        var command = new AcceptAnswerCommand(1, 1);

        //Act
        var result = await _acceptAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_AnswerAlreadyAccepted()
    {
        //Arrange
        var command = new AcceptAnswerCommand(2, 3);

        //Act
        var result = await _acceptAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerAlreadyAccepted, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}