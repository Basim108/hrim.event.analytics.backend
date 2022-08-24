using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

public class BaseEventCreateRequestValidator<TEvent>: AbstractValidator<TEvent>
    where TEvent : BaseEventCreateRequest {
    protected BaseEventCreateRequestValidator() {
        RuleFor(x => x.EventTypeId)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);
    }
}