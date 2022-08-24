using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

public class BaseEventUpdateRequestValidator<TEvent>: EntityUpdateRequestValidator<TEvent>
    where TEvent : BaseEventUpdateRequest {
    protected BaseEventUpdateRequestValidator() {
        RuleFor(x => x.EventTypeId)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);
    }
}