using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class DurationEventUpdateRequestValidator : AbstractValidator<DurationEventUpdateRequest>
{
    /// <summary> </summary>
    public DurationEventUpdateRequestValidator()
    {
        this.AddRulesForEntityUpdateRequests();
        this.AddRulesForBaseEvent();
        this.AddRulesForDurationEvent();
    }
}