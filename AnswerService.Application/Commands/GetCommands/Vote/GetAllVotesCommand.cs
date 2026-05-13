using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Vote;

public record GetAllVotesCommand : IRequest<QueryableResult<Domain.Entities.Vote>>;