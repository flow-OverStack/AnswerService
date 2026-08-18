using AnswerService.Application.Enums;
using AnswerService.Application.Extensions;
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
        var answers = await answerRepository.GetAll()
            .Where(x => request.Ids.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        if (answers.Length == 0) return CollectionResult<Domain.Entities.Answer>.AnswersNotFound(request.Ids.Count);

        return CollectionResult<Domain.Entities.Answer>.Success(answers);
    }
}