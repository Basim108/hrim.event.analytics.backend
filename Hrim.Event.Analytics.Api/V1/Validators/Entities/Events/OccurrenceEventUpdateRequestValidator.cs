using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class OccurrenceEventUpdateRequestValidator: AbstractValidator<OccurrenceEventUpdateRequest>
{
    /// <summary> </summary>
    public OccurrenceEventUpdateRequestValidator() {
        this.AddRulesForEntityUpdateRequests<OccurrenceEventUpdateRequest, long>();
        this.AddRulesForBaseEvent();
        this.AddRulesForOccurrenceEvent();
    }
}