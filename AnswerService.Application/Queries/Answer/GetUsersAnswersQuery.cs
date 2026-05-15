using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Answer;

public record GetUsersAnswersQuery(IEnumerable<long> UserIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>;