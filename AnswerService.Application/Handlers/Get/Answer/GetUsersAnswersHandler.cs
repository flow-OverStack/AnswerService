using AnswerService.Application.Commands.GetCommands.Answer;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Answer;

public class GetUsersAnswersHandler(IBaseRepository<Domain.Entities.Answer> answerRepository)
    : IRequestHandler<GetUsersAnswersCommand, CollectionResult<Domain.Entities.Answer>>
{
    public async Task<CollectionResult<Domain.Entities.Answer>> Handle(GetUsersAnswersCommand request,
        CancellationToken cancellationToken)
    {
        var userIds = request.UserIds.ToArray();
        var answers = await answerRepository.GetAll()
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Answer>.Success(answers);
    }
}