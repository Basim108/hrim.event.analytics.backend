using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities;

/// <summary>
/// Rules for Duration Event related validators
/// </summary>
public static class EventValidatorExtensions {
    /// <summary>
    /// Adds rules to update and create duration event requests
    /// </summary>
    public static void AddRulesForEntityUpdateRequests<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : HrimEntity {
        validator.RuleFor(x => x.Id)
                 .NotEmpty()
                 .WithMessage(ValidationMessages.IS_REQUIRED);

        validator.RuleFor(x => x.ConcurrentToken)
                 .GreaterThan(0);
    }
    
    /// <summary>
    /// Adds rules to update and create duration event requests
    /// </summary>
    public static void AddRulesForEntityCreateRequests<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : HrimEntity {
        validator.RuleFor(x => x.Id)
                 .Empty();

        validator.RuleFor(x => x.ConcurrentToken)
                 .Equal(0);
    }
}