using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Vote;

public record GetVotesCommand(IEnumerable<long> Ids) : IRequest<CollectionResult<Domain.Entities.Vote>>;