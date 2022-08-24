using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Events;
using Hrimsoft.StringCases;

namespace Hrim.Event.Analytics.Api.V1.Validators.Events;

/// <inheritdoc />
public class DurationEventUpdateRequestValidator: AbstractValidator<DurationEventUpdateRequest> {
    /// <summary> </summary>
    public DurationEventUpdateRequestValidator() {
       RuleFor(x => x.Id)
         .NotEmpty()
         .WithMessage(ValidationMessages.IS_REQUIRED);
       
        RuleFor(x => x.EventTypeId)
           .NotEmpty()
           .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.StartedAt)
          .GreaterThan(DateTimeOffset.MinValue)
          .WithMessage(ValidationMessages.IS_REQUIRED);

        RuleFor(x => x.FinishedAt)
          .GreaterThan(DateTimeOffset.MinValue)
          .WithMessage(ValidationMessages.IS_REQUIRED)
          .When(y => y.FinishedAt.HasValue);

        RuleFor(x => x.FinishedAt)
          .GreaterThan(x => x.StartedAt)
          .WithMessage(ValidationMessages.GREATER_THAN_PROPERTY
                                         .Replace(ValidationMessages.PROPERTY_NAME_TEMPLATE,
                                                  nameof(DurationEvent.StartedAt).ToSnakeCase()));
        
        RuleFor(x => x.ConcurrentToken)
           .GreaterThan(0);
    }
}