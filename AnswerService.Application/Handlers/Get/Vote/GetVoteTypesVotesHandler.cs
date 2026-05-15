using AnswerService.Application.Commands.GetCommands.Vote;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetVoteTypesVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetVoteTypesVotesCommand, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>> Handle(
        GetVoteTypesVotesCommand request,
        CancellationToken cancellationToken)
    {
        var voteTypeIds = request.VoteTypeIds.ToArray();

        var votes = (await voteRepository.GetAll()
                .Where(x => voteTypeIds.Contains(x.VoteTypeId))
                .GroupBy(x => x.VoteTypeId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>(x.Key, x.ToArray()))
            .ToArray();

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Success(votes);
    }
}