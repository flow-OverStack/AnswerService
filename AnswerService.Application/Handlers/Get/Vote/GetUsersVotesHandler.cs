using AnswerService.Application.Queries.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetUsersVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetUsersVotesQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>> Handle(
        GetUsersVotesQuery request,
        CancellationToken cancellationToken)
    {
        var userIds = request.UserIds.ToArray();

        var votes = (await voteRepository.GetAll()
                .Where(x => userIds.Contains(x.UserId))
                .GroupBy(x => x.UserId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>(x.Key, x.ToArray()))
            .ToArray();

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Success(votes);
    }
}