using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrimsoft.StringCases;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <summary>
///     Rules for event related validators
/// </summary>
public static class EventValidatorExtensions
{
    /// <summary>
    ///     Adds rules to update and create base event requests
    /// </summary>
    public static void AddRulesForBaseEvent<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : BaseEvent {
        validator.RuleFor(x => x.EventTypeId)
                 .NotEmpty()
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED);
    }

    /// <summary>
    ///     Adds rules to update and create duration event requests
    /// </summary>
    public static void AddRulesForDurationEvent<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : DurationEvent {
        validator.RuleFor(x => x.StartedAt)
                 .GreaterThan(valueToCompare: DateTimeOffset.MinValue)
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED);

        validator.RuleFor(x => x.FinishedAt)
                 .GreaterThan(valueToCompare: DateTimeOffset.MinValue)
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED)
                 .When(y => y.FinishedAt.HasValue);

        validator.RuleFor(x => x.FinishedAt)
                 .GreaterThan(x => x.StartedAt)
                 .WithMessage(ValidationMessages.GREATER_THAN_PROPERTY
                                                .Replace(oldValue: ValidationMessages.PROPERTY_NAME_TEMPLATE,
                                                         nameof(DurationEvent.StartedAt).ToSnakeCase()));
    }

    /// <summary>
    ///     Adds rules to update and create occurrence event requests
    /// </summary>
    public static void AddRulesForOccurrenceEvent<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : OccurrenceEvent {
        validator.RuleFor(x => x.OccurredAt)
                 .GreaterThan(valueToCompare: DateTimeOffset.MinValue)
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED);
    }
}