using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Vote;

public record GetUsersVotesCommand(IEnumerable<long> UserIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>;