using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.Vote;

public class CacheGetUsersVotesHandler(
    IVoteCacheRepository cacheRepository,
    IRequestHandler<GetUsersVotesQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>> inner)
    : IRequestHandler<GetUsersVotesQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>> Handle(
        GetUsersVotesQuery request,
        CancellationToken cancellationToken)
    {
        var idsArray = request.UserIds.ToArray();
        var votes = (await cacheRepository.GetUsersVotesAsync(idsArray,
            async (idsToFetch, ct) => (await inner.Handle(new GetUsersVotesQuery(idsToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (votes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Failure(
                ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Success(votes);
    }
}