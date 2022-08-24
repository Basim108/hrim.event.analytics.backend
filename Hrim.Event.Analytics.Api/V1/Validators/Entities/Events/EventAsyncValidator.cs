using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Services;
using MediatR;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.Events;

/// <summary>
/// Checks event lookups
/// </summary>
public class EventAsyncValidator: AbstractValidator<BaseEvent> {
    /// <inheritdoc />
    public EventAsyncValidator(IMediator mediator, IApiRequestAccessor requestAccessor) {
        RuleFor(x => x.EventTypeId)
           .MustAsync(async (id, cancellationToken) => {
                var cqrsResult = await mediator.Send(new CheckEntityExistence(id, EntityType.EventType, requestAccessor.GetCorrelationId()),
                                                     cancellationToken);
                return cqrsResult.StatusCode == CqrsResultCode.Ok;
            })
           .WithMessage(ValidationMessages.ENTITY_DOES_NOT_EXISTS);
    }
}