using AnswerService.Application.Commands.GetCommands.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetVoteTypesVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetVoteTypesVotesCommand, CollectionResult<Domain.Entities.Vote>>
{
    public async Task<CollectionResult<Domain.Entities.Vote>> Handle(GetVoteTypesVotesCommand request,
        CancellationToken cancellationToken)
    {
        var voteTypeIds = request.VoteTypeIds.ToArray();
        var votes = await voteRepository.GetAll()
            .Where(x => voteTypeIds.Contains(x.VoteTypeId))
            .ToListAsync(cancellationToken);

        return CollectionResult<Domain.Entities.Vote>.Success(votes);
    }
}