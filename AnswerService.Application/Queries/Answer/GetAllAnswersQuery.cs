using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Answer;

public record GetAllAnswersQuery : IRequest<QueryableResult<Domain.Entities.Answer>>;