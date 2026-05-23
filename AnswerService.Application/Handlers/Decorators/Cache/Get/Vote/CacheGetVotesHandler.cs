using AnswerService.Application.Enum;
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
        var keys = request.Dtos.ToArray();
        var votes = (await cacheRepository.GetByDtosAsync(keys,
            async (dtosToFetch, ct) => (await inner.Handle(new GetVotesQuery(dtosToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (votes.Length == 0)
            return keys.Length switch
            {
                <= 1 => CollectionResult<Domain.Entities.Vote>.Failure(ErrorMessage.VoteNotFound,
                    (int)ErrorCodes.VoteNotFound),
                > 1 => CollectionResult<Domain.Entities.Vote>.Failure(ErrorMessage.VotesNotFound,
                    (int)ErrorCodes.VotesNotFound)
            };

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}