using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetUsersVotesQuery(IReadOnlyCollection<long> UserIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>;