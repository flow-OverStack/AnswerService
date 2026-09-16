using AnswerService.Application.Enums;
using AnswerService.Application.Helpers;
using AnswerService.Domain.Results;
using FluentValidation;
using MediatR;

namespace AnswerService.Application.Behaviours;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BaseResult
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next(cancellationToken);

        var validationTasks = validators.Select(v => v.ValidateAsync(request, cancellationToken));
        var validations = await Task.WhenAll(validationTasks);

        if (validations.All(x => x.IsValid)) return await next(cancellationToken);

        var errors = validations.Where(x => !x.IsValid).SelectMany(x => x.Errors);
        var errorMessage = string.Join(", ", errors);

        return ResultFactory.Failure<TResponse>(errorMessage, (int)ErrorCodes.InvalidProperty);
    }
}