using AnswerService.Domain.Dtos.Answer;
using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.AnswerCommands;

public record RevokeAcceptanceCommand(long Id, long InitiatorId) : IRequest<BaseResult<AnswerDto>>;