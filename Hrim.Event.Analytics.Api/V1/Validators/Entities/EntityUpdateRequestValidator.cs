using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities;
#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities; 

public class EntityUpdateRequestValidator<TUpdateRequest>: AbstractValidator<TUpdateRequest>
where TUpdateRequest: EntityUpdateRequest {
    protected EntityUpdateRequestValidator() {
        RuleFor(x => x.Id)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.ConcurrentToken)
           .GreaterThan(0);
    }
}