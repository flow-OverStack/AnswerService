using AnswerService.Application.Queries.Answer;
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

        return CollectionResult<Domain.Entities.Answer>.Success(answers);
    }
}