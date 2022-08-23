using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators;

/// <summary>
/// Validates requests for entity controller
/// </summary>
public class EntityRequestValidator: AbstractValidator<EntityRequest> {
    /// <summary> </summary>
    public EntityRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);
    }
}