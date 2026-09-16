using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Answer;

public record GetQuestionsAnswersQuery(IReadOnlyCollection<long> QuestionIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>;