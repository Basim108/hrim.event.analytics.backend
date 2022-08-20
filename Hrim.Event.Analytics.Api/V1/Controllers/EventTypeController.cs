using System.Net;
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
    public Task<IList<ViewSystemEventType>> GetAllAsync(CancellationToken cancellationToken)
        => _mediator.Send(new GetAllViewEventTypes(_requestAccessor.GetCorrelationId()),
                          cancellationToken);

    /// <summary> Get a duration event type by id</summary>
    [HttpGet("duration/{id}")]
    public async Task<ActionResult<DurationEventType>> GetDurationByIdAsync(Guid id, CancellationToken cancellationToken) {
        var result = await _mediator.Send(new GetDurationEventTypeById(id, IsNotTrackable: true, _requestAccessor.GetCorrelationId()),
                                          cancellationToken);
        if (result == null)
            return NotFound();
        if (result.IsDeleted == true) {
            Response.StatusCode = (int)HttpStatusCode.Gone;
            return new EmptyResult();
        }
        return Ok(result);
    }

    /// <summary> Get an occurrence event type by id</summary>
    [HttpGet("occurrence/{id}")]
    public async Task<ActionResult<OccurrenceEventType>> GetOccurrenceByIdAsync(Guid id, CancellationToken cancellationToken) {
        var result = await _mediator.Send(new GetOccurrenceEventTypeById(id, IsNotTrackable: true, _requestAccessor.GetCorrelationId()),
                                          cancellationToken);
        if (result == null)
            return NotFound();
        if (result.IsDeleted == true) {
            Response.StatusCode = (int)HttpStatusCode.Gone;
            return new EmptyResult();
        }
        return Ok(result);
    }

    /// <summary>
    /// Create a user event type based on ony specific system event type, depends on $type field in json
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SystemEventType>> CreateAsync(SystemEventType eventType, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new CreateEventTypeCommand(eventType, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new ObjectResult(cqrsResult.Result);
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.Ok:
            case CqrsResultCode.Created:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<SystemEventType?>(cqrsResult);
    }

    /// <summary> Update a user event type based on ony specific system event type, depends on $type field in json </summary>
    [HttpPut]
    public async Task<ActionResult<SystemEventType>> UpdateAsync(SystemEventType eventType, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new UpdateEventTypeCommand(eventType, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new EmptyResult();
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<SystemEventType?>(cqrsResult);
    }

    /// <summary> Delete an occurrence event type by its id</summary>
    [HttpDelete("occurrence/{id}")]
    public async Task<ActionResult<OccurrenceEventType>> DeleteOccurrenceAsync(Guid id, CancellationToken cancellationToken) {
        var command    = new SoftDeleteEntityCommand<OccurrenceEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
        var cqrsResult = await _mediator.Send(command, cancellationToken);
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
    public async Task<ActionResult<DurationEventType>> DeleteDurationAsync(Guid id, CancellationToken cancellationToken) {
        var command    = new SoftDeleteEntityCommand<DurationEventType>(id, SaveChanges: true, _requestAccessor.GetCorrelationId());
        var cqrsResult = await _mediator.Send(command, cancellationToken);
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