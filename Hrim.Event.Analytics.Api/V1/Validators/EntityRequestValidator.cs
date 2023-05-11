using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators;

/// <summary>
///     Validates requests for entity controller
/// </summary>
public class EntityRequestValidator: AbstractValidator<EntityRequest>
{
    /// <summary> </summary>
    public EntityRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.EntityType)
           .NotNull()
           .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED)
           .IsInEnum();
    }
}