using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands;

public record UpvoteAnswerCommand(long Id, long InitiatorId) : IRequest<BaseResult<VoteAnswerDto>>;