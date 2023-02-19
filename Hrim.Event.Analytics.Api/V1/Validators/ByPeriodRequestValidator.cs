using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;

namespace Hrim.Event.Analytics.Api.V1.Validators;

/// <summary>
///     Validates by period requests
/// </summary>
public class ByPeriodRequestValidator : AbstractValidator<ByPeriodRequest>
{
    /// <summary> </summary>
    public ByPeriodRequestValidator()
    {
        RuleFor(x => x.Start)
            .GreaterThan(DateOnly.MinValue)
            .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.End)
            .GreaterThan(DateOnly.MinValue)
            .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.End)
            .GreaterThan(x => x.Start)
            .WithMessage(ValidationMessages.GREATER_THAN_PROPERTY
                .Replace(ValidationMessages.PROPERTY_NAME_TEMPLATE,
                    nameof(ByPeriodRequest.Start).ToSnakeCase()));
    }
}