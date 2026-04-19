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
    public async Task Handle_ShouldBe_Success()
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
    public async Task Handle_ShouldBe_Success_With_DownvoteGiven()
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
    public async Task Handle_ShouldBe_UserNotFound()
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
    public async Task Handle_ShouldBe_AnswerNotFound()
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
    public async Task Handle_ShouldBe_CannotVoteForOwnPost()
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
    public async Task Handle_ShouldBe_VoteTypeNotFound()
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
    public async Task Handle_ShouldBe_TooLowReputation()
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
    public async Task Handle_ShouldBe_VoteAlreadyGiven()
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