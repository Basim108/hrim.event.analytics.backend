using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event/duration")]
public class EventDurationController: EventAnalyticsApiController {
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EventDurationController(IApiRequestAccessor requestAccessor,
                                   IMediator           mediator): base(requestAccessor) {
        _mediator = mediator;
    }

    /// <summary> Create a duration event </summary>
    [HttpPost]
    public async Task<ActionResult<DurationEvent>> CreateDurationAsync(DurationEventCreateRequest request, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new DurationEventCreateCommand(request, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut]
    public async Task<ActionResult<DurationEvent>> UpdateDurationAsync(DurationEventUpdateRequest request, CancellationToken cancellationToken) {
        var cqrsResult = await _mediator.Send(new DurationEventUpdateCommand(request, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Get user event type by id </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DurationEvent>> GetDurationByIdAsync(Guid id, CancellationToken cancellationToken) {
        var durationResult = await _mediator.Send(new GetEventById<DurationEvent>(id, IsNotTrackable: true, OperationContext),
                                                  cancellationToken);
        return ProcessCqrsResult(durationResult);
    }
}