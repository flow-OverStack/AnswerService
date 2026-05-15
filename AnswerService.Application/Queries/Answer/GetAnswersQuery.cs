using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Answer;

public record GetAnswersQuery(IEnumerable<long> Ids) : IRequest<CollectionResult<Domain.Entities.Answer>>;