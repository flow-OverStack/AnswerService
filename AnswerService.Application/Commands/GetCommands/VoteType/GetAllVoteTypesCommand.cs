using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.VoteType;

public record GetAllVoteTypesCommand : IRequest<QueryableResult<Domain.Entities.VoteType>>;