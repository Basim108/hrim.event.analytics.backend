using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.ModelBinders;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event")]
public class EventController: EventAnalyticsApiController {
    private readonly IApiRequestAccessor _requestAccessor;
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public EventController(IApiRequestAccessor requestAccessor,
                           IMediator           mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Get user's events for a period </summary>
    [HttpGet]
    public async Task<EventResponse> GetUserEventsAsync(DateOnly start,
                                                        DateOnly end, 
                                                        [FromQuery(Name = "owner_id")]
                                                        Guid ownerId,
                                                        CancellationToken cancellationToken) {
        var occurrences = await _mediator.Send(new GetUserOccurrencesForPeriod(start, end, ownerId, _requestAccessor.GetCorrelationId()),
                                               cancellationToken);
        var durations = await _mediator.Send(new GetUserDurationsForPeriod(start, end, ownerId, _requestAccessor.GetCorrelationId()),
                                             cancellationToken);
        return new EventResponse(new EventRequest(start, end, ownerId), occurrences, durations);
    }

    /// <summary> Create an occurrence event </summary>
    [HttpPost("occurrence")]
    public async Task<ActionResult<OccurrenceEvent>> CreateOccurrenceAsync(OccurrenceEvent occurrence, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new OccurrenceEventCreateCommand(occurrence, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        return ProcessCreateResult(cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut("occurrence")]
    public async Task<ActionResult<OccurrenceEvent>> UpdateOccurrenceAsync(OccurrenceEvent eventToUpdate, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new OccurrenceEventUpdateCommand(eventToUpdate, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        return ProcessUpdateResult(cqrsResult);
    }

    /// <summary> Create a duration event </summary>
    [HttpPost("duration")]
    public async Task<ActionResult<DurationEvent>> CreateDurationAsync(DurationEvent duration, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new DurationEventCreateCommand(duration, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        return ProcessCreateResult(cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut("duration")]
    public async Task<ActionResult<DurationEvent>> UpdateDurationAsync(DurationEvent eventToUpdate, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new DurationEventUpdateCommand(eventToUpdate, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        return ProcessUpdateResult(cqrsResult);
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
        return ProcessGetByIdResult(eventInstance);
    }
}