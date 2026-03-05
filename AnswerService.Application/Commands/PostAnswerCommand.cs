using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Interfaces.Validation;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands;

public record PostAnswerCommand(string Body, long UserId, long QuestionId)
    : IRequest<BaseResult<AnswerDto>>, IValidatableAnswer;