using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Manage any entity type </summary>
[ApiController]
#if RELEASE
[Authorize]
#endif
[Route("v1/entity/{id}")]
public class EntityController: EventAnalyticsApiController {
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EntityController(IApiRequestAccessor requestAccessor,
                            IMediator           mediator): base(requestAccessor) {
        _mediator = mediator;
    }

    /// <summary> Restore a soft deleted instance of any entity</summary>
    [HttpPatch]
    public async Task<ActionResult<HrimEntity>> RestoreAsync([FromRoute] EntityRequest entityRequest, CancellationToken cancellationToken) {
        CqrsResultCode? resultCode;
        HrimEntity?     result;
        switch (entityRequest.EntityType) {
            case EntityType.EventType:
                var restoreEventType = new RestoreEntityCommand<UserEventType>(entityRequest.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<UserEventType>, UserEventType>(restoreEventType, cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var restoreOccurrence = new RestoreEntityCommand<OccurrenceEvent>(entityRequest.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<OccurrenceEvent>, OccurrenceEvent>(restoreOccurrence, cancellationToken);
                break;
            case EntityType.DurationEvent:
                var restoreDuration = new RestoreEntityCommand<DurationEvent>(entityRequest.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<DurationEvent>, DurationEvent>(restoreDuration, cancellationToken);
                break;
            case EntityType.HrimUser:
                var restoreUser = new RestoreEntityCommand<HrimUser>(entityRequest.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<HrimUser>, HrimUser>(restoreUser, cancellationToken);
                break;
            case EntityType.HrimTag:
                var restoreTag = new RestoreEntityCommand<HrimTag>(entityRequest.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<RestoreEntityCommand<HrimTag>, HrimTag>(restoreTag, cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + entityRequest.EntityType);
        }
        return resultCode switch {
            CqrsResultCode.Forbidden => StatusCode((int)HttpStatusCode.Forbidden,
                                                   ApiLogs.FORBID_AS_NOT_ENTITY_OWNER),
            CqrsResultCode.EntityIsNotDeleted => Ok(result),
            CqrsResultCode.NotFound           => NotFound(),
            CqrsResultCode.Ok                 => Ok(result),
            _                                 => throw new UnexpectedCqrsStatusCodeException(resultCode)
        };
    }

    /// <summary> Soft-delete an instance of any entity</summary>
    [HttpDelete]
    public async Task<ActionResult<HrimEntity>> SoftDeleteAsync([FromRoute] EntityRequest request, CancellationToken cancellationToken) {
        CqrsResultCode? resultCode;
        HrimEntity?     result;
        switch (request.EntityType) {
            case EntityType.EventType:
                var deleteEventType = new SoftDeleteEntityCommand<UserEventType>(request.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<UserEventType>, UserEventType>(deleteEventType, cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var deleteOccurrence = new SoftDeleteEntityCommand<OccurrenceEvent>(request.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<OccurrenceEvent>, OccurrenceEvent>(deleteOccurrence, cancellationToken);
                break;
            case EntityType.DurationEvent:
                var deleteDuration = new SoftDeleteEntityCommand<DurationEvent>(request.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<DurationEvent>, DurationEvent>(deleteDuration, cancellationToken);
                break;
            case EntityType.HrimUser:
                var deleteUser = new SoftDeleteEntityCommand<HrimUser>(request.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<HrimUser>, HrimUser>(deleteUser, cancellationToken);
                break;
            case EntityType.HrimTag:
                var deleteTag = new SoftDeleteEntityCommand<HrimTag>(request.Id, SaveChanges: true, OperationContext);
                (resultCode, result) = await InvokeDeletionAsync<SoftDeleteEntityCommand<HrimTag>, HrimTag>(deleteTag, cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + request.EntityType);
        }
        switch (resultCode) {
            case CqrsResultCode.Forbidden:
                return StatusCode((int)HttpStatusCode.Forbidden, ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new EmptyResult();
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(result);
            default:
                throw new UnexpectedCqrsStatusCodeException(resultCode);
        }
    }

    private async Task<(CqrsResultCode ResultCode, HrimEntity? result)> InvokeDeletionAsync<TCommand, TEntity>(TCommand          command,
                                                                                                               CancellationToken cancellationToken)
        where TEntity : HrimEntity, new()
        where TCommand : IRequest<CqrsResult<TEntity?>> {
        var userResult = await _mediator.Send(command, cancellationToken);
        return (userResult.StatusCode, userResult.Result);
    }
}