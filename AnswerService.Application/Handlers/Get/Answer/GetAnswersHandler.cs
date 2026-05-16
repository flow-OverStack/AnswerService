using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Answer;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Answer;

public class GetAnswersHandler(IBaseRepository<Domain.Entities.Answer> answerRepository)
    : IRequestHandler<GetAnswersQuery, CollectionResult<Domain.Entities.Answer>>
{
    public async Task<CollectionResult<Domain.Entities.Answer>> Handle(GetAnswersQuery request,
        CancellationToken cancellationToken)
    {
        var ids = request.Ids.ToArray();
        var answers = await answerRepository.GetAll()
            .Where(x => ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (answers.Length == 0)
            return ids.Length switch
            {
                <= 1 => CollectionResult<Domain.Entities.Answer>.Failure(ErrorMessage.AnswerNotFound,
                    (int)ErrorCodes.AnswerNotFound),
                > 1 => CollectionResult<Domain.Entities.Answer>.Failure(ErrorMessage.AnswersNotFound,
                    (int)ErrorCodes.AnswersNotFound)
            };

        return CollectionResult<Domain.Entities.Answer>.Success(answers);
    }
}