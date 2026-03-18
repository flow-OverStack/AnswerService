using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Interfaces.Validation;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.AnswerCommands;

public record EditAnswerCommand(long Id, string Body, long InitiatorId)
    : IRequest<BaseResult<AnswerDto>>, IValidatableAnswer;