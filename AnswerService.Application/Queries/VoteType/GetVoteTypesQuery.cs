using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.VoteType;

public record GetVoteTypesQuery(IEnumerable<long> VoteTypeIds) : IRequest<CollectionResult<Domain.Entities.VoteType>>;