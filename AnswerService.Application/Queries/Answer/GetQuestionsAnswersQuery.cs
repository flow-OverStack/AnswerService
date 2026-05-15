using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Answer;

public record GetQuestionsAnswersQuery(IEnumerable<long> QuestionIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>;