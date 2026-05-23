using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.Vote;

public class CacheGetAnswersVotesHandler(
    IVoteCacheRepository cacheRepository,
    IRequestHandler<GetAnswersVotesQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>
        inner)
    : IRequestHandler<GetAnswersVotesQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>> Handle(
        GetAnswersVotesQuery request,
        CancellationToken cancellationToken)
    {
        var idsArray = request.AnswerIds.ToArray();
        var votes = (await cacheRepository.GetAnswersVotesAsync(idsArray,
            async (idsToFetch, ct) => (await inner.Handle(new GetAnswersVotesQuery(idsToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (votes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Failure(
                ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Success(votes);
    }
}