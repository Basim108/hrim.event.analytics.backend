using Hrim.Event.Analytics.Abstractions.EventTypes;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Models.EventTypes;
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
    public Task<IEnumerable<SystemEventType>> GetAllAsync(CancellationToken cancellation)
        => _mediator.Send(new GetAllPublicUserEventTypes(_requestAccessor.GetCorrelationId()),
                          cancellation);

    /// <summary> Delete a user event type by its id</summary>
    [HttpGet("{id}")]
    public Task<SystemEventType> GetByIdAsync(Guid id, CancellationToken cancellation) 
        => _mediator.Send(new GetUserEventTypeById(id, _requestAccessor.GetCorrelationId()),
                          cancellation);
    

    /// <summary>
    /// Create a user event type based on ony specific system event type, depends on $type field in json
    /// </summary>
    [HttpPost]
    public Task<SystemEventType> CreateAsync(SystemEventType eventType, CancellationToken cancellation) 
    => _mediator.Send(new CreateUserEventType(eventType, _requestAccessor.GetCorrelationId()),
                      cancellation);

    /// <summary> Update a user event type based on ony specific system event type, depends on $type field in json </summary>
    [HttpPut]
    public Task<SystemEventType> UpdateAsync(OccurrenceEventType eventType, CancellationToken cancellation)
        => _mediator.Send(new UpdateUserEventType(eventType, _requestAccessor.GetCorrelationId()),
                          cancellation);

    /// <summary> Delete a user event type by its id</summary>
    [HttpDelete("{id}")]
    public Task<NoContentResult> DeleteAsync(Guid id, CancellationToken cancellation) {
        return Task.FromResult(NoContent());
    }
}