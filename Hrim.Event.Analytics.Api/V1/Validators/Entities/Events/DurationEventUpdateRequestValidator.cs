using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.StringCases;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class DurationEventUpdateRequestValidator: AbstractValidator<DurationEventUpdateRequest> {
    /// <summary> </summary>
    public DurationEventUpdateRequestValidator() {
        this.AddRulesForEntityUpdateRequests();
        this.AddRulesForBaseEvent();
        this.AddRulesForDurationEvent();
    }
}