using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Vote;

public record GetVoteTypesVotesCommand(IEnumerable<long> VoteTypeIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Vote>>>>;