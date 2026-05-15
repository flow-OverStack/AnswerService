using AnswerService.Application.Settings;
using AnswerService.Domain.Dto.Page;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AnswerService.Application.Validation;

public class OffsetPageDtoValidator : AbstractValidator<OffsetPageDto>
{
    public OffsetPageDtoValidator(IOptions<PaginationRules> pagination)
    {
        var maxPageSize = pagination.Value.MaxPageSize;

        RuleFor(x => x.Skip)
            .NotNull().WithMessage($"'{nameof(OffsetPageDto.Skip)}' must be provided.")
            .GreaterThanOrEqualTo(0).WithMessage($"'{nameof(OffsetPageDto.Skip)}' must be greater than or equal to 0.");

        RuleFor(x => x.Take)
            .NotNull().WithMessage($"'{nameof(OffsetPageDto.Take)}' must be provided.")
            .InclusiveBetween(0, maxPageSize)
            .WithMessage($"'{nameof(OffsetPageDto.Take)}' must be between 0 and {maxPageSize}.");
    }
}