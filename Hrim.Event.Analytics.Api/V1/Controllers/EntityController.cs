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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Manage any entity type </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    public async Task<ActionResult<HrimEntity<long>>> RestoreAsync([FromRoute] EntityRequest<long> entityRequest,
                                                                   CancellationToken               cancellationToken) {
        var               operationContext = _requestAccessor.GetOperationContext();
        CqrsResultCode?   resultCode;
        HrimEntity<long>? result;
        switch (entityRequest.EntityType) {
            case EntityType.EventType:
                var restoreEventType = new RestoreLongEntityCommand<EventType>(Id: entityRequest.Id,
                                                                               SaveChanges: true,
                                                                               Context: operationContext);
                (resultCode, result) = await InvokeDeletionAsync<RestoreLongEntityCommand<EventType>, EventType>(command: restoreEventType,
                                                                                                                 cancellationToken: cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var restoreOccurrence =
                    new RestoreLongEntityCommand<OccurrenceEvent>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreLongEntityCommand<OccurrenceEvent>, OccurrenceEvent>(command: restoreOccurrence,
                                                                                                          cancellationToken: cancellationToken);
                break;
            case EntityType.DurationEvent:
                var restoreDuration = new RestoreLongEntityCommand<DurationEvent>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreLongEntityCommand<DurationEvent>, DurationEvent>(command: restoreDuration,
                                                                                                      cancellationToken: cancellationToken);
                break;
            case EntityType.HrimUser:
                var restoreUser = new RestoreLongEntityCommand<HrimUser>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreLongEntityCommand<HrimUser>, HrimUser>(command: restoreUser, cancellationToken: cancellationToken);
                break;
            case EntityType.HrimTag:
                var restoreTag = new RestoreLongEntityCommand<HrimTag>(Id: entityRequest.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<RestoreLongEntityCommand<HrimTag>, HrimTag>(command: restoreTag, cancellationToken: cancellationToken);
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
    public async Task<ActionResult<HrimEntity<long>>> SoftDeleteAsync([FromRoute] EntityRequest<long> request,
                                                                      CancellationToken               cancellationToken) {
        var operationContext = _requestAccessor.GetOperationContext();

        CqrsResultCode? resultCode;
        HrimEntity<long>?     result;
        switch (request.EntityType) {
            case EntityType.EventType:
                var deleteEventType = new SoftDeleteLongEntityCommand<EventType>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteLongEntityCommand<EventType>, EventType>(command: deleteEventType,
                                                                                                 cancellationToken: cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                var deleteOccurrence = new SoftDeleteLongEntityCommand<OccurrenceEvent>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteLongEntityCommand<OccurrenceEvent>, OccurrenceEvent>(
                                                                                                             command: deleteOccurrence,
                                                                                                             cancellationToken: cancellationToken);
                break;
            case EntityType.DurationEvent:
                var deleteDuration = new SoftDeleteLongEntityCommand<DurationEvent>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteLongEntityCommand<DurationEvent>, DurationEvent>(command: deleteDuration,
                                                                                                         cancellationToken: cancellationToken);
                break;
            case EntityType.HrimUser:
                var deleteUser = new SoftDeleteLongEntityCommand<HrimUser>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteLongEntityCommand<HrimUser>, HrimUser>(command: deleteUser,
                                                                                               cancellationToken: cancellationToken);
                break;
            case EntityType.HrimTag:
                var deleteTag = new SoftDeleteLongEntityCommand<HrimTag>(Id: request.Id, SaveChanges: true, Context: operationContext);
                (resultCode, result) =
                    await InvokeDeletionAsync<SoftDeleteLongEntityCommand<HrimTag>, HrimTag>(command: deleteTag, cancellationToken: cancellationToken);
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

    private async Task<(CqrsResultCode ResultCode, HrimEntity<long>? result)> InvokeDeletionAsync<TCommand, TEntity>(TCommand          command,
                                                                                                                     CancellationToken cancellationToken)
        where TEntity : HrimEntity<long>, new()
        where TCommand : IRequest<CqrsResult<TEntity?>> {
        var userResult = await _mediator.Send(request: command, cancellationToken: cancellationToken);
        return (userResult.StatusCode, userResult.Result);
    }
}