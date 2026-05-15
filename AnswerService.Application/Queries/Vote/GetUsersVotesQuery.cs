using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetUsersVotesQuery(IEnumerable<long> UserIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>;