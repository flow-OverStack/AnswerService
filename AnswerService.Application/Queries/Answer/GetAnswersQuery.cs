using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Answer;

public record GetAnswersQuery(IReadOnlyCollection<long> Ids) : IRequest<CollectionResult<Domain.Entities.Answer>>;