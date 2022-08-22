using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Api.ModelBinders;
using Hrim.Event.Analytics.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event")]
public class EventController: ControllerBase {
    private readonly IApiRequestAccessor _requestAccessor;
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public EventController(IApiRequestAccessor requestAccessor,
                           IMediator           mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Create an occurrence event </summary>
    [HttpPost("occurrence")]
    public async Task<ActionResult<OccurrenceEvent>> CreateOccurrenceAsync(OccurrenceEvent occurrence, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new CreateOccurrenceEventCommand(occurrence, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.BadRequest:
                return BadRequest(JsonConvert.SerializeObject(cqrsResult.Info));
            case CqrsResultCode.Ok:
            case CqrsResultCode.Created:
                return Ok(cqrsResult.Result);
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new ObjectResult(cqrsResult.Result);
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<OccurrenceEvent?>(cqrsResult);
    }

    /// <summary> Create an occurrence event </summary>
    [HttpPost("duration")]
    public async Task<ActionResult<DurationEvent>> CreateDurationAsync(DurationEvent duration, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new CreateDurationEventCommand(duration, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.BadRequest:
                return BadRequest(JsonConvert.SerializeObject(cqrsResult.Info));
            case CqrsResultCode.Ok:
            case CqrsResultCode.Created:
                return Ok(cqrsResult.Result);
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new ObjectResult(cqrsResult.Result);
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<DurationEvent?>(cqrsResult);
    }
    
    /// <summary> Get user event type by id </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BaseEvent>> GetByIdAsync(Guid id,
                                                            [FromQuery(Name = "eventType")] [ModelBinder(typeof(JsonModelBinder<EntityType>))]
                                                            EntityType entityType,
                                                            CancellationToken cancellationToken) {
        var        correlationId = _requestAccessor.GetCorrelationId();
        BaseEvent? eventInstance;
        switch (entityType) {
            case EntityType.OccurrenceEvent:
                eventInstance = await _mediator.Send(new GetEventById<OccurrenceEvent>(id, IsNotTrackable: true, correlationId),
                                                     cancellationToken);
                break;
            case EntityType.DurationEvent:
                eventInstance = await _mediator.Send(new GetEventById<DurationEvent>(id, IsNotTrackable: true, correlationId),
                                                     cancellationToken);
                break;
            default:
                return BadRequest("Unsupported entity: " + entityType);
        }
        if (eventInstance == null)
            return NotFound();
        if (eventInstance.IsDeleted == true) {
            Response.StatusCode = (int)HttpStatusCode.Gone;
            return new EmptyResult();
        }
        return Ok(eventInstance);
    }
}