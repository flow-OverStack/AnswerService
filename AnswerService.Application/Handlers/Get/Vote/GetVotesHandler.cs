using AnswerService.Application.Commands.GetCommands.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetVotesCommand, CollectionResult<Domain.Entities.Vote>>
{
    public async Task<CollectionResult<Domain.Entities.Vote>> Handle(GetVotesCommand request,
        CancellationToken cancellationToken)
    {
        var ids = request.Ids.ToArray();
        var votes = await voteRepository.GetAll()
            .Where(x => ids.Contains(x.AnswerId))
            .ToListAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}