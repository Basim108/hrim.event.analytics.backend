using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Filters;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
///     Manage user event types with this crud controller
/// </summary>
[ApiController]
[Route("v1/event/duration")]
public class EventDurationController : EventBaseController<DurationEvent>
{
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EventDurationController(IApiRequestAccessor requestAccessor,
        IValidator<DurationEvent> validator,
        IMediator mediator)
        : base(requestAccessor, validator, mediator)
    {
        _mediator = mediator;
    }

    /// <summary> Get duration events for a period </summary>
    [HttpGet]
    public async Task<ActionResult<EventsForPeriodResponse>> GetForPeriodAsync([FromQuery] ByPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var query = new DurationEventGetForPeriod(request.Start, request.End, OperationContext);
        var durations = await _mediator.Send(query, cancellationToken);
        return new EventsForPeriodResponse(new GetEventsForPeriodRequest(request.Start, request.End), null, durations);
    }
    
    /// <summary> Create a duration event </summary>
    [HttpPost]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<DurationEvent>> CreateDurationAsync(DurationEventCreateRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateRequestAsync(request, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var cqrsResult = await _mediator.Send(new DurationEventCreateCommand(request, true, OperationContext),
            cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<DurationEvent>> UpdateDurationAsync(DurationEventUpdateRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateRequestAsync(request, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var cqrsResult = await _mediator.Send(new DurationEventUpdateCommand(request, true, OperationContext),
            cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }
}