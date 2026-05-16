using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Answer;

public class GetUsersAnswersHandler(IBaseRepository<Domain.Entities.Answer> answerRepository)
    : IRequestHandler<GetUsersAnswersQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>> Handle(
        GetUsersAnswersQuery request,
        CancellationToken cancellationToken)
    {
        var userIds = request.UserIds.ToArray();

        var answers = (await answerRepository.GetAll()
                .Where(x => userIds.Contains(x.UserId))
                .GroupBy(x => x.UserId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>(x.Key, x.ToArray()))
            .ToArray();

        if (answers.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Failure(
                ErrorMessage.AnswersNotFound, (int)ErrorCodes.AnswersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>.Success(answers);
    }
}