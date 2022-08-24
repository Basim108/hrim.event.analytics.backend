using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class OccurrenceEventUpdateRequestValidator: BaseEventUpdateRequestValidator<OccurrenceEventUpdateRequest> {
    /// <summary> </summary>
    public OccurrenceEventUpdateRequestValidator() {
        RuleFor(x => x.OccurredAt)
           .GreaterThan(DateTimeOffset.MinValue)
          .WithMessage(ValidationMessages.IS_REQUIRED);
    }
}