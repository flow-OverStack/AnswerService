using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetVoteTypesVotesQuery(IReadOnlyCollection<long> VoteTypeIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>;