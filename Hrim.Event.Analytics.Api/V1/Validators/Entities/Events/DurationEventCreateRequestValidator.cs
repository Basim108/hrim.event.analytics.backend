using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrimsoft.StringCases;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class DurationEventCreateRequestValidator: BaseEventCreateRequestValidator<DurationEventCreateRequest> {
    /// <summary> </summary>
    public DurationEventCreateRequestValidator() {
        RuleFor(x => x.StartedAt)
           .GreaterThan(DateTimeOffset.MinValue)
           .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.FinishedAt)
           .GreaterThan(DateTimeOffset.MinValue)
           .WithMessage(ValidationMessages.IS_REQUIRED)
           .When(y => y.FinishedAt.HasValue);

        RuleFor(x => x.FinishedAt)
           .GreaterThan(x => x.StartedAt)
           .WithMessage(ValidationMessages.GREATER_THAN_PROPERTY
                                          .Replace(ValidationMessages.PROPERTY_NAME_TEMPLATE,
                                                   nameof(DurationEvent.StartedAt).ToSnakeCase()));
    }
}