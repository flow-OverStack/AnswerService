using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Answer;

public record GetAllAnswersCommand : IRequest<QueryableResult<Domain.Entities.Answer>>;