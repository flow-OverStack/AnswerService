using AnswerService.Domain.Dtos.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.AnswerCommands;

public record UpvoteAnswerCommand(long Id, long InitiatorId) : IRequest<BaseResult<VoteAnswerDto>>;