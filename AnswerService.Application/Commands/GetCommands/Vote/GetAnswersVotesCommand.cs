using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Vote;

public record GetAnswersVotesCommand(IEnumerable<long> AnswerIds) : IRequest<CollectionResult<Domain.Entities.Vote>>;