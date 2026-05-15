using AnswerService.Domain.Dto.Vote;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetVotesQuery(IEnumerable<VoteDto> Dtos) : IRequest<CollectionResult<Domain.Entities.Vote>>;