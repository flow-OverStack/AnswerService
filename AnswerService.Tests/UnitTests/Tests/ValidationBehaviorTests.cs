using AnswerService.Application.Behaviours;
using AnswerService.Application.Commands.AnswerCommands;
using AnswerService.Application.Resources;
using AnswerService.Application.Validation;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Interfaces.Validation;
using AnswerService.Domain.Results;
using AnswerService.Tests.UnitTests.Fixtures;
using MediatR;
using Moq;
using Xunit;
using AnswerService.Tests.Traits;

namespace AnswerService.Tests.UnitTests.Tests;

[UnitTest]
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

    [Fact]
    public async Task Handle_NoValidatorsRegistered_ReturnsSuccess()
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

    [Fact]
    public async Task Handle_ValidCommandWithValidator_ReturnsSuccess()
    {
        //Arrange
        var behavior = new ValidationBehavior<PostAnswerCommand, AnswerDto>([
            ValidatorFixture<IValidatableAnswer>.GetValidator(new AnswerValidator())
        ]);
        var command = new PostAnswerCommand(ValidBody, 1, 1);

        //Act
        var result = await behavior.Handle(command, _mockNext.Object, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Handle_EmptyAnswerBody_ReturnsInvalidAnswerBody()
    {
        //Arrange
        var behavior = new ValidationBehavior<PostAnswerCommand, AnswerDto>([
            ValidatorFixture<IValidatableAnswer>.GetValidator(new AnswerValidator())
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