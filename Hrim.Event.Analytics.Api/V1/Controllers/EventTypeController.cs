using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using Hrim.Event.Analytics.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event-type")]
public class EventTypeController: ControllerBase {
    private readonly IApiRequestAccessor _requestAccessor;
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public EventTypeController(IApiRequestAccessor requestAccessor,
                               IMediator           mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Get all user event types </summary>
    [HttpGet]
    public Task<IEnumerable<ViewSystemEventType>> GetAllAsync(CancellationToken cancellation)
        => _mediator.Send(new GetAllPublicViewEventTypes(_requestAccessor.GetCorrelationId()),
                          cancellation);

    /// <summary> Get a duration event type by id</summary>
    [HttpGet("duration/{id}")]
    public async Task<ActionResult<DurationEventType>> GetDurationByIdAsync(Guid id, CancellationToken cancellation) {
        var result = await _mediator.Send(new GetDurationEventTypeById(id, IsNotTrackable: true, _requestAccessor.GetCorrelationId()),
                                          cancellation);
        if (result == null || result.IsDeleted == true)
            return NotFound();
        return Ok(result);
    }

    /// <summary> Get an occurrence event type by id</summary>
    [HttpGet("occurrence/{id}")]
    public async Task<ActionResult<OccurrenceEventType>> GetOccurrenceByIdAsync(Guid id, CancellationToken cancellation) {
        var result = await _mediator.Send(new GetOccurrenceEventTypeById(id, IsNotTrackable: true, _requestAccessor.GetCorrelationId()),
                                          cancellation);
        if (result == null || result.IsDeleted == true)
            return NotFound();
        return Ok(result);
    }
}