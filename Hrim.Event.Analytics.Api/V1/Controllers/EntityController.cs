using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
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
    
    /// <summary> Delete an occurrence event type by its id</summary>
    [HttpPatch("{id}")]
    public async Task<ActionResult<Entity>> RestoreAsync(Guid id, 
                                                         [FromQuery(Name="entityType")] 
                                                         [ModelBinder(typeof(JsonModelBinder<EntityType>))]
                                                         EntityType entityType, 
                                                         CancellationToken cancellationToken) {
        CqrsResultCode? resultCode;
        Entity? result;
        switch(entityType) {
            case EntityType.OccurrenceEventType:
                var restoreOccurrence = new RestoreEntityCommand<OccurrenceEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                var occurenceResult        = await _mediator.Send(restoreOccurrence, cancellationToken);
                resultCode = occurenceResult.StatusCode;
                result     = occurenceResult.Result;
                break;
            case EntityType.DurationEventType:
                var restoreDuration = new RestoreEntityCommand<DurationEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
                var durationResult      = await _mediator.Send(restoreDuration, cancellationToken);
                resultCode = durationResult.StatusCode;
                result     = durationResult.Result;
                break;
            default:
                return NotFound("Unsupported entity: " + entityType);
        }
        return resultCode switch {
            CqrsResultCode.EntityIsNotDeleted => Ok(result),
            CqrsResultCode.NotFound           => NotFound(),
            CqrsResultCode.Ok                 => Ok(result),
            _                                 => throw new UnexpectedCqrsStatusCodeException(resultCode)
        };
    }

}