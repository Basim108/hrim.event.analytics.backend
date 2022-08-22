using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Api.ModelBinders;
using Hrim.Event.Analytics.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Manage any entity type </summary>
[ApiController]
[Route("v1/entity")]
public class EntityController: ControllerBase {
    private readonly IApiRequestAccessor _requestAccessor;
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public EntityController(IApiRequestAccessor requestAccessor,
                            IMediator           mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Restore a soft deleted instance of any entity</summary>
    [HttpPatch("{id}")]
    public async Task<ActionResult<HrimEntity>> RestoreAsync(Guid id,
                                                         [FromQuery(Name = "entityType")] [ModelBinder(typeof(JsonModelBinder<EntityType>))]
                                                         EntityType entityType,
                                                         CancellationToken cancellationToken) {
        CqrsResultCode? resultCode;
        HrimEntity?         result;
        switch (entityType) {
            case EntityType.EventType:
                var restoreEventType = new RestoreEntityCommand<UserEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<UserEventType>, UserEventType>(restoreEventType, cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var restoreOccurrence = new RestoreEntityCommand<OccurrenceEvent>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<OccurrenceEvent>, OccurrenceEvent>(restoreOccurrence, cancellationToken);
                break;
            case EntityType.DurationEvent:
                var restoreDuration = new RestoreEntityCommand<DurationEvent>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<DurationEvent>, DurationEvent>(restoreDuration, cancellationToken);
                break;
            case EntityType.HrimUser:
                var restoreUser = new RestoreEntityCommand<HrimUser>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<HrimUser>, HrimUser>(restoreUser, cancellationToken);
                break;
            case EntityType.HrimTag:
                var restoreTag = new RestoreEntityCommand<HrimTag>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<HrimTag>, HrimTag>(restoreTag, cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + entityType);
        }
        return resultCode switch {
            CqrsResultCode.EntityIsNotDeleted => Ok(result),
            CqrsResultCode.NotFound           => NotFound(),
            CqrsResultCode.Ok                 => Ok(result),
            _                                 => throw new UnexpectedCqrsStatusCodeException(resultCode)
        };
    }

    /// <summary> Soft-delete an instance of any entity</summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<HrimEntity>> SoftDeleteAsync(Guid id,
                                                            [FromQuery(Name = "entityType")] [ModelBinder(typeof(JsonModelBinder<EntityType>))]
                                                            EntityType entityType,
                                                            CancellationToken cancellationToken) {
        CqrsResultCode? resultCode;
        HrimEntity?         result;
        switch (entityType) {
            case EntityType.EventType:
                var deleteEventType = new SoftDeleteEntityCommand<UserEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<UserEventType>, UserEventType>(deleteEventType, cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var deleteOccurrence = new SoftDeleteEntityCommand<OccurrenceEvent>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<OccurrenceEvent>, OccurrenceEvent>(deleteOccurrence, cancellationToken);
                break;
            case EntityType.DurationEvent:
                var deleteDuration = new SoftDeleteEntityCommand<DurationEvent>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<DurationEvent>, DurationEvent>(deleteDuration, cancellationToken);
                break;
            case EntityType.HrimUser:
                var deleteUser = new SoftDeleteEntityCommand<HrimUser>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<HrimUser>, HrimUser>(deleteUser, cancellationToken);
                break;
            case EntityType.HrimTag:
                var deleteTag = new SoftDeleteEntityCommand<HrimTag>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<HrimTag>, HrimTag>(deleteTag, cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + entityType);
        }
        switch (resultCode) {
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new EmptyResult();
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(result);
        }
        throw new UnexpectedCqrsStatusCodeException(resultCode);
    }

    private async Task<(CqrsResultCode ResultCode, HrimEntity? result)> InvokeDeletionAsync<TCommand, TEntity>(TCommand command, 
                                                                                                           CancellationToken cancellationToken)
        where TEntity : HrimEntity, new()
        where TCommand : IRequest<CqrsResult<TEntity?>> {
        var userResult = await _mediator.Send(command, cancellationToken);
        return (userResult.StatusCode, userResult.Result);
    }
}