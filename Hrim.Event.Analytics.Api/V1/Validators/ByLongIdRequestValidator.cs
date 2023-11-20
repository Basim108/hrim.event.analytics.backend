using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators;

/// <summary>
///     Validates requests for entity controller
/// </summary>
public class ByLongIdRequestValidator: AbstractValidator<ByIdRequest<long>> 
{
    /// <summary> </summary>
    public ByLongIdRequestValidator() {
        RuleFor(x => x.Id)
           .GreaterThan(0)
           .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED);
    }
}