using FluentValidation;
using Hrim.Event.Analytics.Api.V1.Models;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;

/// <inheritdoc />
public class CreateEventTypeRequestValidator: AbstractValidator<CreateEventTypeRequest>
{
    /// <summary> </summary>
    public CreateEventTypeRequestValidator() {
        this.AddRulesForEntityCreateRequests<CreateEventTypeRequest, long>();
        this.AddRulesForEventType();
    }
}