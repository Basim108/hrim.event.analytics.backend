using System.Text.RegularExpressions;
using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;

/// <summary>
///     Rules for event type related validators
/// </summary>
public static class EventTypeValidatorExtensions
{
    /// <summary>
    ///     Adds rules to update and create event_type requests
    /// </summary>
    public static void AddRulesForEventType<TRequest>(this AbstractValidator<TRequest> validator)
        where TRequest : UserEventType
    {
        validator.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.IS_REQUIRED)
            .MaximumLength(Constraints.NAME_MAX_LENGTH);

        validator.RuleFor(x => x.Description)
            .MaximumLength(Constraints.DESCRIPTION_MAX_LENGTH);

        validator.RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage(ValidationMessages.IS_REQUIRED)
            .MaximumLength(Constraints.COLOR_MAX_LENGTH);

        validator.RuleFor(x => x.Color)
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.IgnoreCase)
            .WithMessage(ValidationMessages.IS_REQUIRED)
            .MaximumLength(Constraints.COLOR_MAX_LENGTH);
    }
}