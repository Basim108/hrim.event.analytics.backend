using System.Text.RegularExpressions;
using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.V1.Validators.Entities.Analysis;

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
        where TRequest : EventType {
        validator.RuleFor(x => x.Name)
                 .NotEmpty()
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED)
                 .MaximumLength(maximumLength: Constraints.NAME_MAX_LENGTH);

        validator.RuleFor(x => x.Description)
                 .MaximumLength(maximumLength: Constraints.DESCRIPTION_MAX_LENGTH);

        validator.RuleFor(x => x.Color)
                 .NotEmpty()
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED)
                 .MaximumLength(maximumLength: Constraints.COLOR_MAX_LENGTH);

        validator.RuleFor(x => x.Color)
                 .Matches(expression: "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", options: RegexOptions.IgnoreCase)
                 .WithMessage(errorMessage: ValidationMessages.IS_REQUIRED)
                 .MaximumLength(maximumLength: Constraints.COLOR_MAX_LENGTH);
        
        validator.RuleForEach(x => x.AnalysisSettings)
                 .SetValidator(new AnalysisByEventTypeValidator());
    }
}