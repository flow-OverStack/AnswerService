using AnswerService.Application.Commands.GetCommands.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetAllVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetAllVotesCommand, QueryableResult<Domain.Entities.Vote>>
{
    public Task<QueryableResult<Domain.Entities.Vote>> Handle(GetAllVotesCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var votes = voteRepository.GetAll();

        return Task.FromResult(QueryableResult<Domain.Entities.Vote>.Success(votes));
    }
}