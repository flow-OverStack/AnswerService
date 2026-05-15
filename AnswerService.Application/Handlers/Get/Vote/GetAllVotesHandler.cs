using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetAllVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetAllVotesQuery, QueryableResult<Domain.Entities.Vote>>
{
    public Task<QueryableResult<Domain.Entities.Vote>> Handle(GetAllVotesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var votes = voteRepository.GetAll();

        return Task.FromResult(QueryableResult<Domain.Entities.Vote>.Success(votes));
    }
}