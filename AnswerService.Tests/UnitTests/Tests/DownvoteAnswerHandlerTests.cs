using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Domain.Entities;
using AnswerService.Tests.Mocks;
using AnswerService.Tests.UnitTests.Fixtures;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
public class DownvoteAnswerHandlerTests
{
    private readonly DownvoteAnswerHandler _downvoteAnswerHandler = new(
        RepositoryMocks.GetMockUnitOfWork().Object,
        RepositoryMocks.GetMockVoteTypeRepository().Object,
        EntityProviderMocks.GetMockUserProvider().Object,
        BaseEventProducerFixture.GetBaseEventProducerConfiguration(),
        MapperFixture.GetMapperConfiguration());

    [Fact]
    public async Task Handle_NoExistingVote_ReturnsSuccess()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(3, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_ExistingUpvote_ReturnsSuccess()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(2, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentUserId_ReturnsUserNotFound()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(1, 0);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NonExistentAnswerId_ReturnsAnswerNotFound()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(0, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_OwnAnswer_ReturnsCannotVoteForOwnPost()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(1, 1);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotVoteForOwnPost, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_NoVoteTypesConfigured_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var downvoteAnswerHandler = new DownvoteAnswerHandler(
            RepositoryMocks.GetMockUnitOfWork().Object,
            RepositoryMocks.GetEmptyMockRepository<VoteType>().Object,
            EntityProviderMocks.GetMockUserProvider().Object,
            BaseEventProducerFixture.GetBaseEventProducerConfiguration(),
            MapperFixture.GetMapperConfiguration());
        var command = new DownvoteAnswerCommand(1, 3);

        //Act
        var result = await downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_LowReputationUser_ReturnsTooLowReputation()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(1, 4);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ExistingDownvote_ReturnsVoteAlreadyGiven()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(1, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}