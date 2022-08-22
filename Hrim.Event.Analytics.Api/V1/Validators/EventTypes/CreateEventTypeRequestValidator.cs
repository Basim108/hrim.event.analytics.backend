using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;

namespace Hrim.Event.Analytics.Api.V1.Validators.EventTypes;

/// <inheritdoc />
public class CreateEventTypeRequestValidator: AbstractValidator<CreateEventTypeRequest> {
    const int NAME_MAX_LENGTH        = 128;
    const int DESCRIPTION_MAX_LENGTH = 2000;
    const int COLOR_MAX_LENGTH       = 7;

    /// <summary> </summary>
    public CreateEventTypeRequestValidator() {
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired)
           .MaximumLength(NAME_MAX_LENGTH);

        RuleFor(x => x.Description)
           .MaximumLength(DESCRIPTION_MAX_LENGTH);

        RuleFor(x => x.Color)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired)
           .MaximumLength(NAME_MAX_LENGTH);

        RuleFor(x => x.Color)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired)
          .MaximumLength(COLOR_MAX_LENGTH);
        
        RuleFor(x => x.EventType)
          .NotEmpty()
          .WithMessage(ValidationMessages.IsRequired);
    }
}