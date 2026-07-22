using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.UnitTests.Fixtures;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class RemoveVoteHandlerTests
{
    private readonly RemoveVoteHandler _removeVoteHandler = new(
        RepositoryMocks.GetMockUnitOfWork().Object,
        EntityProviderMocks.GetMockUserProvider().Object,
        BaseEventProducerFixture.GetBaseEventProducerConfiguration(),
        MapperFixture.GetMapperConfiguration()
    );

    [Fact]
    public async Task Handle_ExistingVote_ReturnsSuccess()
    {
        //Arrange
        var command = new RemoveVoteCommand(1, 3);

        //Act
        var result = await _removeVoteHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentUserId_ReturnsUserNotFound()
    {
        //Arrange
        var command = new RemoveVoteCommand(1, 0);

        //Act
        var result = await _removeVoteHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentAnswerId_ReturnsAnswerNotFound()
    {
        //Arrange
        var command = new RemoveVoteCommand(0, 3);

        //Act
        var result = await _removeVoteHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NoExistingVote_ReturnsVoteNotFound()
    {
        //Arrange
        var command = new RemoveVoteCommand(3, 3);

        //Act
        var result = await _removeVoteHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}