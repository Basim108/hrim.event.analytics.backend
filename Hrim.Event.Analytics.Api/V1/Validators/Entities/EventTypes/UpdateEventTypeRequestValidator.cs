using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;

/// <inheritdoc />
public class UpdateEventTypeRequestValidator: AbstractValidator<UpdateEventTypeRequest> {
    /// <summary> </summary>
    public UpdateEventTypeRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);
        
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED)
           .MaximumLength(Constraints.NAME_MAX_LENGTH);

        RuleFor(x => x.Description)
           .MaximumLength(Constraints.DESCRIPTION_MAX_LENGTH);

        RuleFor(x => x.Color)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED)
           .MaximumLength(Constraints.NAME_MAX_LENGTH);

        RuleFor(x => x.Color)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED)
           .MaximumLength(Constraints.COLOR_MAX_LENGTH);

        RuleFor(x => x.ConcurrentToken)
           .GreaterThan(0);
    }
}