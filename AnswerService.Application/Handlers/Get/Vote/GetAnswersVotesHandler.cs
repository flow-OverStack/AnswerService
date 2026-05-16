using AnswerService.Application.Enum;
using AnswerService.Application.Queries.Vote;
using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Repository;
using AnswerService.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Application.Handlers.Get.Vote;

public class GetAnswersVotesHandler(IBaseRepository<Domain.Entities.Vote> voteRepository)
    : IRequestHandler<GetAnswersVotesQuery, CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>
{
    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>> Handle(
        GetAnswersVotesQuery request,
        CancellationToken cancellationToken)
    {
        var answerIds = request.AnswerIds.ToArray();

        var votes = (await voteRepository.GetAll()
                .Where(x => answerIds.Contains(x.AnswerId))
                .GroupBy(x => x.AnswerId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>(x.Key, x.ToArray()))
            .ToArray();

        if (votes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Failure(
                ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>.Success(votes);
    }
}