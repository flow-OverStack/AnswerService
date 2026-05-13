using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Answer;

public record GetAnswersCommand(IEnumerable<long> Ids) : IRequest<CollectionResult<Domain.Entities.Answer>>;