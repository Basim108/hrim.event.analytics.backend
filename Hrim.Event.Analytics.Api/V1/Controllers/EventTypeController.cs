using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event-type")]
public class EventTypeController: EventAnalyticsApiController {
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
    public Task<IList<ViewEventType>> GetAllAsync(CancellationToken cancellationToken)
        => _mediator.Send(new GetViewEventTypes(_requestAccessor.GetCorrelationId()),
                          cancellationToken);

    /// <summary> Get user event type by id </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserEventType>> GetByIdAsync([FromRoute]ByIdRequest request, CancellationToken cancellationToken) {
        var result = await _mediator.Send(new GetEventTypeById(request.Id, IsNotTrackable: true, _requestAccessor.GetCorrelationId()),
                                    cancellationToken);
        return ProcessGetByIdResult(result);
    }

    /// <summary> Create a new event type </summary>
    [HttpPost]
    public async Task<ActionResult<UserEventType>> CreateAsync(UserEventType eventType, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new CreateUserEventTypeCommand(eventType, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        return ProcessCreateResult(cqrsResult);
    }

    /// <summary> Update an event type </summary>
    [HttpPut]
    public async Task<ActionResult<UserEventType>> UpdateAsync(UserEventType eventType, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new UpdateEventTypeCommand(eventType, SaveChanges: true, _requestAccessor.GetCorrelationId()),
                                              cancellationToken);
        return ProcessUpdateResult(cqrsResult);
    }
}