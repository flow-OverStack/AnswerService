using AnswerService.Application.Enum;
using AnswerService.Application.Queries.VoteType;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.VoteType;

public class CacheGetVoteTypesHandler(
    IVoteTypeCacheRepository cacheRepository,
    IRequestHandler<GetVoteTypesQuery, CollectionResult<Domain.Entities.VoteType>> inner)
    : IRequestHandler<GetVoteTypesQuery, CollectionResult<Domain.Entities.VoteType>>
{
    public async Task<CollectionResult<Domain.Entities.VoteType>> Handle(GetVoteTypesQuery request,
        CancellationToken cancellationToken)
    {
        var idsArray = request.VoteTypeIds.ToArray();
        var voteTypes = (await cacheRepository.GetByIdsAsync(idsArray,
            async (idsToFetch, ct) => (await inner.Handle(new GetVoteTypesQuery(idsToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (voteTypes.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<Domain.Entities.VoteType>.Failure(ErrorMessage.VoteTypeNotFound,
                    (int)ErrorCodes.VoteTypeNotFound),
                > 1 => CollectionResult<Domain.Entities.VoteType>.Failure(ErrorMessage.VoteTypesNotFound,
                    (int)ErrorCodes.VoteTypesNotFound)
            };

        return CollectionResult<Domain.Entities.VoteType>.Success(voteTypes);
    }
}