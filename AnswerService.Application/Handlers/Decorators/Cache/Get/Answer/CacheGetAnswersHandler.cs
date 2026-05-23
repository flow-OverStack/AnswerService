using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.Answer;

public class CacheGetAnswersHandler(
    IAnswerCacheRepository cacheRepository,
    IRequestHandler<GetAnswersQuery, CollectionResult<Domain.Entities.Answer>> inner)
    : IRequestHandler<GetAnswersQuery, CollectionResult<Domain.Entities.Answer>>
{
    public async Task<CollectionResult<Domain.Entities.Answer>> Handle(GetAnswersQuery request,
        CancellationToken cancellationToken)
    {
        var idsArray = request.Ids.ToArray();
        var questions = (await cacheRepository.GetByIdsAsync(idsArray,
            async (idsToFetch, ct) => (await inner.Handle(new GetAnswersQuery(idsToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (questions.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<Domain.Entities.Answer>.Failure(ErrorMessage.AnswerNotFound,
                    (int)ErrorCodes.AnswerNotFound),
                > 1 => CollectionResult<Domain.Entities.Answer>.Failure(ErrorMessage.AnswersNotFound,
                    (int)ErrorCodes.AnswersNotFound)
            };

        return CollectionResult<Domain.Entities.Answer>.Success(questions);
    }
}