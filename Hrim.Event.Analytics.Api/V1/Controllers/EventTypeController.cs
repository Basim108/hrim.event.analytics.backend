using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
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
    public Task<IList<ViewSystemEventType>> GetAllAsync(CancellationToken cancellation)
        => _mediator.Send(new GetAllViewEventTypes(_requestAccessor.GetCorrelationId()),
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
    
    /// <summary>
    /// Create a user event type based on ony specific system event type, depends on $type field in json
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SystemEventType>> CreateAsync(SystemEventType eventType, CancellationToken cancellation) {
        var cqrsResult = await _mediator.Send(new CreateEventTypeCommand(eventType, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellation);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.Ok:
            case CqrsResultCode.Created:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<SystemEventType?>(cqrsResult);
    }

    /// <summary> Delete an occurrence event type by its id</summary>
    [HttpDelete("occurrence/{id}")]
    public async Task<ActionResult<OccurrenceEventType>> DeleteOccurrenceAsync(Guid id, CancellationToken cancellation) {
        var command    = new SoftDeleteEntityCommand<OccurrenceEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
        var cqrsResult = await _mediator.Send(command, cancellation);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<OccurrenceEventType?>(cqrsResult);
    }

    /// <summary> Delete an duration event type by its id</summary>
    [HttpDelete("duration/{id}")]
    public async Task<ActionResult<DurationEventType>> DeleteDurationAsync(Guid id, CancellationToken cancellation) {
        var command    = new SoftDeleteEntityCommand<DurationEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
        var cqrsResult = await _mediator.Send(command, cancellation);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<DurationEventType?>(cqrsResult);
    }
}