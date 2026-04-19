using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class EditAnswerHandlerTests
{
    private readonly EditAnswerHandler _editAnswerHandler = new(
        MockRepositoriesGetters.GetMockAnswerRepository().Object,
        MockEntityProvidersGetters.GetMockUserProvider().Object,
        MapperConfiguration.GetMapperConfiguration());

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success()
    {
        //Arrange
        var command = new EditAnswerCommand(1, "Edited Answer", 1);

        //Act
        var result = await _editAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_UserNotFound()
    {
        //Arrange
        var command = new EditAnswerCommand(1, "Edited Answer", 0);

        //Act
        var result = await _editAnswerHandler.Handle(command, CancellationToken.None);

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
        var command = new EditAnswerCommand(0, "Edited Answer", 1);

        //Act
        var result = await _editAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_OperationForbidden()
    {
        //Arrange
        var command = new EditAnswerCommand(1, "Edited Answer", 2);

        //Act
        var result = await _editAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}