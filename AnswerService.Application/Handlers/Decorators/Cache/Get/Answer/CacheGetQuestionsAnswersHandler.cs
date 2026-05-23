using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository.Cache;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Decorators.Cache.Get.Answer;

public class CacheGetQuestionsAnswersHandler(
    IAnswerCacheRepository cacheRepository,
    IRequestHandler<GetQuestionsAnswersQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>
        inner)
    : IRequestHandler<GetQuestionsAnswersQuery,
        CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>> Handle(
        GetQuestionsAnswersQuery request,
        CancellationToken cancellationToken)
    {
        var idsArray = request.QuestionIds.ToArray();
        var answers = (await cacheRepository.GetQuestionsAnswersAsync(idsArray,
            async (idsToFetch, ct) => (await inner.Handle(new GetQuestionsAnswersQuery(idsToFetch), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (answers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Failure(
                ErrorMessage.AnswersNotFound,
                (int)ErrorCodes.AnswersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Success(answers);
    }
}