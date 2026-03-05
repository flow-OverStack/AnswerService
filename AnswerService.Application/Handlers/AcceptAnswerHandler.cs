using AnswerService.Application.Commands;
using AnswerService.Domain.Dto.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers;

public class AcceptAnswerHandler : IRequestHandler<AcceptAnswerCommand, BaseResult<AnswerDto>>
{
    public Task<BaseResult<AnswerDto>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}