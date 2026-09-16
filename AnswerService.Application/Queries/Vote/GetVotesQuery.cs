using AnswerService.Domain.Dtos.Vote;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetVotesQuery(IReadOnlyCollection<VoteKey> Keys) : IRequest<CollectionResult<Domain.Entities.Vote>>;