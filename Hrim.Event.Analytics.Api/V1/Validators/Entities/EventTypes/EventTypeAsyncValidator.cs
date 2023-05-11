using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using MediatR;

namespace Hrim.Event.Analytics.Api.V1.Validators.Entities.EventTypes;

/// <summary>
///     Checks event lookups
/// </summary>
public class EventTypeAsyncValidator: AbstractValidator<UserEventType>
{
    /// <inheritdoc />
    public EventTypeAsyncValidator(IMediator mediator, IApiRequestAccessor requestAccessor) {
        RuleFor(x => x.CreatedById)
           .MustAsync(async (id, cancellationToken) => {
                var cqrsResult = await mediator.Send(
                                                     new CheckEntityExistence(Id: id, EntityType: EntityType.HrimUser, requestAccessor.GetCorrelationId()),
                                                     cancellationToken: cancellationToken);
                return cqrsResult.StatusCode == CqrsResultCode.Ok;
            })
           .WithMessage(errorMessage: ValidationMessages.ENTITY_DOES_NOT_EXISTS);
    }
}