using AnswerService.Application.Resources;
using AnswerService.Domain.Interfaces.Validation;
using AnswerService.Domain.Settings;
using FluentValidation;

namespace AnswerService.Application.Validation;

public class AnswerValidator : AbstractValidator<IValidatableAnswer>
{
    public AnswerValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage(ErrorMessage.InvalidAnswerBody)
            .MinimumLength(BusinessRules.BodyMinLength).WithMessage(ErrorMessage.InvalidAnswerBody)
            .MaximumLength(BusinessRules.BodyMaxLength).WithMessage(ErrorMessage.InvalidAnswerBody);
    }
}