using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities;

/// <summary>
///     Rules for Duration Event related validators
/// </summary>
public static class EventValidatorExtensions
{
    /// <summary>
    ///     Adds rules to update and create duration event requests
    /// </summary>
    public static void AddRulesForEntityUpdateRequests<TRequest, TKey>(this AbstractValidator<TRequest> validator)
        where TKey : struct
        where TRequest : HrimEntity<TKey> {
        validator.RuleFor(x => x.Id)
                 .NotEmpty()
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED);

        validator.RuleFor(x => x.ConcurrentToken)
                 .GreaterThan(valueToCompare: 0);
    }

    /// <summary>
    ///     Adds rules to update and create duration event requests
    /// </summary>
    public static void AddRulesForEntityCreateRequests<TRequest, TKey>(this AbstractValidator<TRequest> validator)
        where TKey : struct
        where TRequest : HrimEntity<TKey> {
        validator.RuleFor(x => x.Id)
                 .Empty();

        validator.RuleFor(x => x.ConcurrentToken)
                 .Equal(toCompare: 0);
    }
}