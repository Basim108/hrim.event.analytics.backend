using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;

namespace Hrim.Event.Analytics.Api.V1.Validators.EventTypes;

/// <inheritdoc />
public class UpdateEventTypeRequestValidator: AbstractValidator<UpdateEventTypeRequest> {
    /// <summary> </summary>
    public UpdateEventTypeRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired);
        
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired)
           .MaximumLength(Constraints.NAME_MAX_LENGTH);

        RuleFor(x => x.Description)
           .MaximumLength(Constraints.DESCRIPTION_MAX_LENGTH);

        RuleFor(x => x.Color)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired)
           .MaximumLength(Constraints.NAME_MAX_LENGTH);

        RuleFor(x => x.Color)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired)
           .MaximumLength(Constraints.COLOR_MAX_LENGTH);

        RuleFor(x => x.ConcurrentToken)
           .GreaterThan(0);
    }
}