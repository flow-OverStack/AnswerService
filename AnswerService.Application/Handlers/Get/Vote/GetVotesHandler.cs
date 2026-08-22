using AnswerService.Application.Enums;
using AnswerService.Application.Extensions;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
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
        var predicate = PredicateBuilder.New<Domain.Entities.Vote>();
        predicate = request.Keys.Aggregate(predicate,
            (current, local) =>
                current.Or(x => x.AnswerId == local.AnswerId && x.UserId == local.UserId));

        var votes = await voteRepository.GetAll()
            .AsNoTracking()
            .AsExpandable()
            .Where(predicate)
            .ToArrayAsync(cancellationToken);

        if (votes.Length == 0) return CollectionResult<Domain.Entities.Vote>.VotesNotFound(request.Keys.Count);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}