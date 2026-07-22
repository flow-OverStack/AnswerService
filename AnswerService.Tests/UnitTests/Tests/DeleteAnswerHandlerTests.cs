using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.UnitTests.Fixtures;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class DeleteAnswerHandlerTests
{
    private readonly DeleteAnswerHandler _deleteAnswerHandler = new(
        RepositoryMocks.GetMockUnitOfWork().Object,
        EntityProviderMocks.GetMockUserProvider().Object,
        BaseEventProducerFixture.GetBaseEventProducerConfiguration(),
        MapperFixture.GetMapperConfiguration());

    [Fact]
    public async Task Handle_OwnerDeletesAnswer_ReturnsSuccess()
    {
        //Arrange
        var command = new DeleteAnswerCommand(1, 1);

        //Act
        var result = await _deleteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentUserId_ReturnsUserNotFound()
    {
        //Arrange
        var command = new DeleteAnswerCommand(1, 0);

        //Act
        var result = await _deleteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentAnswerId_ReturnsAnswerNotFound()
    {
        //Arrange
        var command = new DeleteAnswerCommand(0, 1);

        //Act
        var result = await _deleteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NonOwnerUserId_ReturnsForbidden()
    {
        //Arrange
        var command = new DeleteAnswerCommand(1, 2);

        //Act
        var result = await _deleteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}