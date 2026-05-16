using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Answer;

public class GetQuestionsAnswersHandler(IBaseRepository<Domain.Entities.Answer> answerRepository)
    : IRequestHandler<GetQuestionsAnswersQuery,
        CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>> Handle(
        GetQuestionsAnswersQuery request,
        CancellationToken cancellationToken)
    {
        var ids = request.QuestionIds.ToArray();

        var answers = (await answerRepository.GetAll()
                .Where(x => ids.Contains(x.QuestionId))
                .GroupBy(x => x.QuestionId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>(x.Key, x.ToArray()))
            .ToArray();

        if (answers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Failure(
                ErrorMessage.AnswersNotFound,
                (int)ErrorCodes.AnswersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Success(answers);
    }
}