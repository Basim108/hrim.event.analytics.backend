using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class OccurrenceEventCreateRequestValidator: AbstractValidator<OccurrenceEventCreateRequest>
{
    /// <summary> </summary>
    public OccurrenceEventCreateRequestValidator() {
        this.AddRulesForEntityCreateRequests<OccurrenceEventCreateRequest, long>();
        this.AddRulesForBaseEvent();
        this.AddRulesForOccurrenceEvent();
    }
}