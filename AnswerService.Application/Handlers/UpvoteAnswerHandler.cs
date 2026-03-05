using AnswerService.Application.Commands;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers;

public class UpvoteAnswerHandler : IRequestHandler<UpvoteAnswerCommand, BaseResult<VoteAnswerDto>>
{
    public Task<BaseResult<VoteAnswerDto>> Handle(UpvoteAnswerCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}