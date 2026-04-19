using AnswerService.Application.Behaviours;
using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Resources;
using AnswerService.Application.Validation;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Interfaces.Validation;
using AnswerService.Domain.Results;
using AnswerService.Tests.UnitTests.Configurations;
using MediatR;
using Moq;
using Xunit;

namespace AnswerService.Tests.UnitTests.Tests;

public class ValidationBehaviorTests
{
    private const string ValidBody = "Test Body Test Body Test Body ";
    private readonly Mock<RequestHandlerDelegate<BaseResult<AnswerDto>>> _mockNext;

    public ValidationBehaviorTests()
    {
        var mock = new Mock<RequestHandlerDelegate<BaseResult<AnswerDto>>>();
        mock.Setup(x => x.Invoke())
            .ReturnsAsync(
                BaseResult<AnswerDto>.Success(
                    new AnswerDto(1, ValidBody, 1, 1, false)));

        _mockNext = mock;
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success_With_NoValidators()
    {
        //Arrange
        var behavior = new ValidationBehavior<PostAnswerCommand, AnswerDto>([]);
        var command = new PostAnswerCommand(ValidBody, 1, 1);

        //Act
        var result = await behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Success_With_ValidResults()
    {
        //Arrange
        var behavior = new ValidationBehavior<PostAnswerCommand, AnswerDto>([
            ValidatorConfiguration<IValidatableAnswer>.GetValidator(new AnswerValidator())
        ]);
        var command = new PostAnswerCommand(ValidBody, 1, 1);

        //Act
        var result = await behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task Handle_ShouldBe_Failure_With_InvalidResults()
    {
        //Arrange
        var behavior = new ValidationBehavior<PostAnswerCommand, AnswerDto>([
            ValidatorConfiguration<IValidatableAnswer>.GetValidator(new AnswerValidator())
        ]);
        var command = new PostAnswerCommand(string.Empty, 1, 1);

        //Act
        var result = await behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidAnswerBody, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}