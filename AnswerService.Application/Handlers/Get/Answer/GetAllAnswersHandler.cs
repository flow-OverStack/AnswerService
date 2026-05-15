using AnswerService.Application.Queries.Answer;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Handlers.Get.Answer;

public class GetAllAnswersHandler(IBaseRepository<Domain.Entities.Answer> answerRepository)
    : IRequestHandler<GetAllAnswersQuery, QueryableResult<Domain.Entities.Answer>>
{
    public Task<QueryableResult<Domain.Entities.Answer>> Handle(GetAllAnswersQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var answers = answerRepository.GetAll();

        return Task.FromResult(QueryableResult<Domain.Entities.Answer>.Success(answers));
    }
}