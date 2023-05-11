using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Manage any entity type </summary>
[ApiController]
[Route(template: "v1/entity/{id}")]
public class EntityController: ControllerBase
{
    private readonly IMediator           _mediator;
    private readonly IApiRequestAccessor _requestAccessor;

    /// <summary> </summary>
    public EntityController(IApiRequestAccessor requestAccessor,
                            IMediator           mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Restore a soft deleted instance of any entity</summary>
    [HttpPatch]
    public async Task<ActionResult<HrimEntity>> RestoreAsync([FromRoute] EntityRequest entityRequest,
                                                             CancellationToken         cancellationToken) {
        var             operationContext = _requestAccessor.GetOperationContext();
        CqrsResultCode? resultCode;
        HrimEntity?     result;
        switch (entityRequest.EntityType) {
            case EntityType.EventType:
                var restoreEventType =
                    new RestoreEntityCommand<UserEventType>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreEntityCommand<UserEventType>, UserEventType>(command: restoreEventType,
                                                                                                  cancellationToken: cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var restoreOccurrence =
                    new RestoreEntityCommand<OccurrenceEvent>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreEntityCommand<OccurrenceEvent>, OccurrenceEvent>(command: restoreOccurrence,
                                                                                                      cancellationToken: cancellationToken);
                break;
            case EntityType.DurationEvent:
                var restoreDuration = new RestoreEntityCommand<DurationEvent>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreEntityCommand<DurationEvent>, DurationEvent>(command: restoreDuration,
                                                                                                  cancellationToken: cancellationToken);
                break;
            case EntityType.HrimUser:
                var restoreUser = new RestoreEntityCommand<HrimUser>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreEntityCommand<HrimUser>, HrimUser>(command: restoreUser, cancellationToken: cancellationToken);
                break;
            case EntityType.HrimTag:
                var restoreTag = new RestoreEntityCommand<HrimTag>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreEntityCommand<HrimTag>, HrimTag>(command: restoreTag, cancellationToken: cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + entityRequest.EntityType);
        }

        return resultCode switch {
            CqrsResultCode.Forbidden => StatusCode((int)HttpStatusCode.Forbidden,
                                                   value: ApiLogs.FORBID_AS_NOT_ENTITY_OWNER),
            CqrsResultCode.EntityIsNotDeleted => Ok(value: result),
            CqrsResultCode.NotFound           => NotFound(),
            CqrsResultCode.Ok                 => Ok(value: result),
            _                                 => throw new UnexpectedCqrsStatusCodeException(statusCode: resultCode)
        };
    }

    /// <summary> Soft-delete an instance of any entity</summary>
    [HttpDelete]
    public async Task<ActionResult<HrimEntity>> SoftDeleteAsync([FromRoute] EntityRequest request,
                                                                CancellationToken         cancellationToken) {
        var operationContext = _requestAccessor.GetOperationContext();

        CqrsResultCode? resultCode;
        HrimEntity?     result;
        switch (request.EntityType) {
            case EntityType.EventType:
                var deleteEventType = new SoftDeleteEntityCommand<UserEventType>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteEntityCommand<UserEventType>, UserEventType>(command: deleteEventType,
                                                                                                     cancellationToken: cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var deleteOccurrence = new SoftDeleteEntityCommand<OccurrenceEvent>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteEntityCommand<OccurrenceEvent>, OccurrenceEvent>(
                                                                                                         command: deleteOccurrence,
                                                                                                         cancellationToken: cancellationToken);
                break;
            case EntityType.DurationEvent:
                var deleteDuration = new SoftDeleteEntityCommand<DurationEvent>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteEntityCommand<DurationEvent>, DurationEvent>(command: deleteDuration,
                                                                                                     cancellationToken: cancellationToken);
                break;
            case EntityType.HrimUser:
                var deleteUser = new SoftDeleteEntityCommand<HrimUser>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteEntityCommand<HrimUser>, HrimUser>(command: deleteUser,
                                                                                           cancellationToken: cancellationToken);
                break;
            case EntityType.HrimTag:
                var deleteTag = new SoftDeleteEntityCommand<HrimTag>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteEntityCommand<HrimTag>, HrimTag>(command: deleteTag, cancellationToken: cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + request.EntityType);
        }

        switch (resultCode) {
            case CqrsResultCode.Forbidden:
                return StatusCode((int)HttpStatusCode.Forbidden, value: ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new EmptyResult();
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(value: result);
            default:
                throw new UnexpectedCqrsStatusCodeException(statusCode: resultCode);
        }
    }

    private async Task<(CqrsResultCode ResultCode, HrimEntity? result)> InvokeDeletionAsync<TCommand, TEntity>(TCommand          command,
                                                                                                               CancellationToken cancellationToken)
        where TEntity : HrimEntity, new()
        where TCommand : IRequest<CqrsResult<TEntity?>> {
        var userResult = await _mediator.Send(request: command, cancellationToken: cancellationToken);
        return (userResult.StatusCode, userResult.Result);
    }
}