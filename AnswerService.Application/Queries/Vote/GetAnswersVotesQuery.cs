using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Queries.Vote;

public record GetAnswersVotesQuery(IReadOnlyCollection<long> AnswerIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>;