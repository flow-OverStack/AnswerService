using AnswerService.Application.Enums;
using AnswerService.Application.Extensions;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.Vote;

public class CacheGetVotesHandler(
    IVoteCacheRepository cacheRepository,
    IRequestHandler<GetVotesQuery, CollectionResult<Domain.Entities.Vote>> inner)
    : IRequestHandler<GetVotesQuery, CollectionResult<Domain.Entities.Vote>>
{
    public async Task<CollectionResult<Domain.Entities.Vote>> Handle(GetVotesQuery request,
        CancellationToken cancellationToken)
    {
        var keys = request.Keys.ToArray();
        var votes = (await cacheRepository.GetByUserAndAnswerAsync(keys,
            async (keysToFetch, ct) => (await inner.Handle(new GetVotesQuery(keysToFetch.ToArray()), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (votes.Length == 0) return CollectionResult<Domain.Entities.Vote>.VotesNotFound(keys.Length);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}