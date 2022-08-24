using FluentValidation;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class OccurrenceEventUpdateRequestValidator: AbstractValidator<OccurrenceEventUpdateRequest> {
    /// <summary> </summary>
    public OccurrenceEventUpdateRequestValidator() {
        this.AddRulesForEntityUpdateRequests();
        this.AddRulesForBaseEvent();
        this.AddRulesForOccurrenceEvent();
    }
}