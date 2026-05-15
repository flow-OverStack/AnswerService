using AnswerService.Domain.Results;
using MediatR;

namespace AnswerService.Application.Commands.GetCommands.Answer;

public record GetUsersAnswersCommand(IEnumerable<long> UserIds)
    : IRequest<CollectionResult<KeyValuePair<long, IEnumerable<Domain.Entities.Answer>>>>;