using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Events;

namespace Hrim.Event.Analytics.Api.V1.Validators.Events;

/// <inheritdoc />
public class UpdateOccurrenceEventRequestValidator: AbstractValidator<UpdateOccurrenceEventRequest> {
    /// <summary> </summary>
    public UpdateOccurrenceEventRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);
        
        RuleFor(x => x.EventTypeId)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.OccurredAt)
           .GreaterThan(DateTimeOffset.MinValue)
          .WithMessage(ValidationMessages.IS_REQUIRED);
        
        RuleFor(x => x.ConcurrentToken)
           .GreaterThan(0);
    }
}