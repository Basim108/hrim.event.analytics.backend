using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators;

/// <summary>
/// Validates requests for entity controller
/// </summary>
public class ByIdRequestValidator: AbstractValidator<ByIdRequest> {
    /// <summary> </summary>
    public ByIdRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(ValidationMessages.IsRequired);
    }
}