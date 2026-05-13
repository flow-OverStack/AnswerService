using AnswerService.Application.Commands.GetCommands.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetAnswersVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetAnswersVotesCommand, CollectionResult<Domain.Entities.Vote>>
{
    public async Task<CollectionResult<Domain.Entities.Vote>> Handle(GetAnswersVotesCommand request,
        CancellationToken cancellationToken)
    {
        var answerIds = request.AnswerIds.ToArray();
        var votes = await voteRepository.GetAll()
            .Where(x => answerIds.Contains(x.AnswerId))
            .ToListAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}