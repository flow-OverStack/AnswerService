using AnswerService.Application.Commands.GetCommands.Answer;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Answer;

public class GetAnswersHandler(IBaseRepository<Domain.Entities.Answer> answerRepository)
    : IRequestHandler<GetAnswersCommand, CollectionResult<Domain.Entities.Answer>>
{
    public async Task<CollectionResult<Domain.Entities.Answer>> Handle(GetAnswersCommand request,
        CancellationToken cancellationToken)
    {
        var ids = request.Ids.ToArray();
        var answers = await answerRepository.GetAll()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Answer>.Success(answers);
    }
}