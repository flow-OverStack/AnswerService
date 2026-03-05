using AnswerService.Application.Enum;
using AnswerService.Domain.Results;
using FluentValidation;
using MediatR;

namespace AnswerService.Application.Behaviours;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, BaseResult<TResponse>>
    where TRequest : IRequest<BaseResult<TResponse>>
    where TResponse : class
{
    public async Task<BaseResult<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<BaseResult<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next(cancellationToken);

        var validationTasks = validators.Select(v => v.ValidateAsync(request, cancellationToken));
        var validations = await Task.WhenAll(validationTasks);

        if (validations.All(x => x.IsValid)) return await next(cancellationToken);

        var errors = validations.Where(x => !x.IsValid).SelectMany(x => x.Errors);
        var errorMessage = string.Join(", ", errors);

        return BaseResult<TResponse>.Failure(errorMessage, (int)ErrorCodes.InvalidProperty);
    }
}