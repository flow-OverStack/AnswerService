using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetVotesQuery, CollectionResult<Domain.Entities.Vote>>
{
    public async Task<CollectionResult<Domain.Entities.Vote>> Handle(GetVotesQuery request,
        CancellationToken cancellationToken)
    {
        var keys = request.Dtos.ToArray();

        var predicate = PredicateBuilder.New<Domain.Entities.Vote>();
        predicate = keys.Aggregate(predicate,
            (current, local) =>
                current.Or(x => x.AnswerId == local.AnswerId && x.UserId == local.UserId));

        var votes = await voteRepository.GetAll()
            .AsExpandable()
            .Where(predicate)
            .ToArrayAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}