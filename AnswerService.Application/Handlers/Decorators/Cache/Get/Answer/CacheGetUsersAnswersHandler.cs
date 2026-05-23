using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.Answer;

public class CacheGetUsersAnswersHandler(
    IAnswerCacheRepository cacheRepository,
    IRequestHandler<GetUsersAnswersQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>
        inner)
    : IRequestHandler<GetUsersAnswersQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>> Handle(
        GetUsersAnswersQuery request,
        CancellationToken cancellationToken)
    {
        var idsArray = request.UserIds.ToArray();
        var answers = (await cacheRepository.GetUsersAnswersAsync(idsArray,
            async (idsToFetch, ct) => (await inner.Handle(new GetUsersAnswersQuery(idsToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (answers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Failure(
                ErrorMessage.AnswersNotFound, (int)ErrorCodes.AnswersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Success(answers);
    }
}