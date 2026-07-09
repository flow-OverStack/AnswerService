using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Domain.Entities;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class UpvoteAnswerHandlerTests
{
    private readonly UpvoteAnswerHandler _upvoteAnswerHandler = new(
        MockRepositoriesGetters.GetMockUnitOfWork().Object,
        MockRepositoriesGetters.GetMockVoteTypeRepository().Object,
        MockEntityProvidersGetters.GetMockUserProvider().Object,
        BaseEventProducerConfiguration.GetBaseEventProducerConfiguration(),
        MapperConfiguration.GetMapperConfiguration());

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ValidUpvote_ReturnsSuccess()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(3, 3);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ChangingExistingDownvoteToUpvote_ReturnsSuccess()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(1, 3);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsUserNotFound()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(1, 0);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_AnswerDoesNotExist_ReturnsAnswerNotFound()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(0, 2);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.AnswerNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_UserVotesOnOwnAnswer_ReturnsCannotVoteForOwnPost()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(1, 1);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotVoteForOwnPost, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_VoteTypeMissingFromRepository_ReturnsVoteTypeNotFound()
    {
        //Arrange
        var upvoteAnswerHandler = new UpvoteAnswerHandler(
            MockRepositoriesGetters.GetMockUnitOfWork().Object,
            MockRepositoriesGetters.GetEmptyMockRepository<VoteType>().Object,
            MockEntityProvidersGetters.GetMockUserProvider().Object,
            BaseEventProducerConfiguration.GetBaseEventProducerConfiguration(),
            MapperConfiguration.GetMapperConfiguration()
        );
        var command = new UpvoteAnswerCommand(1, 4);

        //Act
        var result = await upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_UserReputationBelowUpvoteThreshold_ReturnsTooLowReputation()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(1, 2);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_UserAlreadyUpvotedAnswer_ReturnsVoteAlreadyGiven()
    {
        //Arrange
        var command = new UpvoteAnswerCommand(2, 3);

        //Act
        var result = await _upvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}