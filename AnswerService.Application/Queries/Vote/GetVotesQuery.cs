using AnswerService.Domain.Dtos.Vote;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetVotesQuery(IReadOnlyCollection<VoteDto> Dtos) : IRequest<CollectionResult<Domain.Entities.Vote>>;