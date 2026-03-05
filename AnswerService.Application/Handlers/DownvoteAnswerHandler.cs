using AnswerService.Application.Commands;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers;

public class DownvoteAnswerHandler : IRequestHandler<DownvoteAnswerCommand, BaseResult<VoteAnswerDto>>
{
    public Task<BaseResult<VoteAnswerDto>> Handle(DownvoteAnswerCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}