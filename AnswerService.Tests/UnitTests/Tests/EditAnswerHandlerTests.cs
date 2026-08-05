using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.UnitTests.Fixtures;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class EditAnswerHandlerTests
{
    private readonly EditAnswerHandler _editAnswerHandler = new(
        RepositoryMocks.GetMockAnswerRepository().Object,
        EntityProviderMocks.GetMockUserProvider().Object,
        MapperFixture.GetMapperConfiguration());

    [Fact]
    public async Task Handle_OwnerEditsAnswer_ReturnsSuccess()
    {
        //Arrange
        var command = new EditAnswerCommand(1, "Edited Answer", 1);

        //Act
        var result = await _editAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentUserId_ReturnsUserNotFound()
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

    [Fact]
    public async Task Handle_NonExistentAnswerId_ReturnsAnswerNotFound()
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

    [Fact]
    public async Task Handle_NonOwnerUserId_ReturnsForbidden()
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