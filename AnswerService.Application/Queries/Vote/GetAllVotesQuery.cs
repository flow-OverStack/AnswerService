using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetAllVotesQuery : IRequest<QueryableResult<Domain.Entities.Vote>>;