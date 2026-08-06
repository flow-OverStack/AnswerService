using AnswerService.Domain.Dtos.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.AnswerCommands;

public record DownvoteAnswerCommand(long Id, long InitiatorId) : IRequest<BaseResult<VoteAnswerDto>>;