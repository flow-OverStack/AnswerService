using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.VoteType;

public record GetAllVoteTypesQuery : IRequest<QueryableResult<Domain.Entities.VoteType>>;