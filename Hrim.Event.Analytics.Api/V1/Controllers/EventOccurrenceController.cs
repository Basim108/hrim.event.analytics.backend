using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event/occurrence")]
public class EventOccurrenceController: EventAnalyticsApiController {
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public EventOccurrenceController(IApiRequestAccessor requestAccessor,
                                     IMediator           mediator): base(requestAccessor) {
        _mediator        = mediator;
    }

    /// <summary> Create an occurrence event </summary>
    [HttpPost]
    public async Task<ActionResult<OccurrenceEvent>> CreateOccurrenceAsync(OccurrenceEventCreateRequest occurrence, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new OccurrenceEventCreateCommand(occurrence, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut]
    public async Task<ActionResult<OccurrenceEvent>> UpdateOccurrenceAsync(OccurrenceEventUpdateRequest eventToOccurrenceEventUpdate, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new OccurrenceEventUpdateCommand(eventToOccurrenceEventUpdate, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Get user event type by id </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OccurrenceEvent>> GetOccurrenceByIdAsync(Guid id, CancellationToken cancellationToken) {
        var occurrenceResult = await _mediator.Send(new GetEventById<OccurrenceEvent>(id, IsNotTrackable: true, OperationContext),
                                                    cancellationToken);
        return ProcessCqrsResult(occurrenceResult);
    }
}