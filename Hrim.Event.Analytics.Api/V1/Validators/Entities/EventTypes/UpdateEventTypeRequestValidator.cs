using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;

/// <inheritdoc />
public class UpdateEventTypeRequestValidator : AbstractValidator<UpdateEventTypeRequest>
{
    /// <summary> </summary>
    public UpdateEventTypeRequestValidator()
    {
        this.AddRulesForEntityUpdateRequests();
        this.AddRulesForEventType();
    }
}