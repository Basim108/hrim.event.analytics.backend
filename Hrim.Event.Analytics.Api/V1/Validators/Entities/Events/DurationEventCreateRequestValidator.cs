using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <inheritdoc />
public class DurationEventCreateRequestValidator: AbstractValidator<DurationEventCreateRequest>
{
    /// <summary> </summary>
    public DurationEventCreateRequestValidator() {
        this.AddRulesForEntityCreateRequests<DurationEventCreateRequest, long>();
        this.AddRulesForBaseEvent();
        this.AddRulesForDurationEvent();
    }
}