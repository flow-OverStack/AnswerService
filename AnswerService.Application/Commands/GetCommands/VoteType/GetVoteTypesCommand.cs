using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.VoteType;

public record GetVoteTypesCommand(IEnumerable<long> VoteTypeIds) : IRequest<CollectionResult<Domain.Entities.VoteType>>;