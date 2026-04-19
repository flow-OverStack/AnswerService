using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Handlers;
using AnswerService.Application.Resources;
using AnswerService.Domain.Entities;
using AnswerService.Tests.Configurations;
using AnswerService.Tests.UnitTests.Configurations;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class DownvoteAnswerHandlerTests
{
    private readonly DownvoteAnswerHandler _downvoteAnswerHandler = new(
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
        var command = new DownvoteAnswerCommand(3, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success_With_UpvoteGiven()
    {
        //Arrange
        var command = new DownvoteAnswerCommand(2, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_UserNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_AnswerNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_CannotVoteForOwnPost()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_VoteTypeNotFound()
    {
        //Arrange
        var downvoteAnswerHandler = new DownvoteAnswerHandler(
            MockRepositoriesGetters.GetMockUnitOfWork().Object,
            MockRepositoriesGetters.GetEmptyMockRepository<VoteType>().Object,
            MockEntityProvidersGetters.GetMockUserProvider().Object,
            BaseEventProducerConfiguration.GetBaseEventProducerConfiguration(),
            MapperConfiguration.GetMapperConfiguration());
        var command = new DownvoteAnswerCommand(1, 3);

        //Act
        var result = await downvoteAnswerHandler.Handle(command, CancellationToken.None);

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
        var command = new DownvoteAnswerCommand(1, 4);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

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
        var command = new DownvoteAnswerCommand(1, 3);

        //Act
        var result = await _downvoteAnswerHandler.Handle(command, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}