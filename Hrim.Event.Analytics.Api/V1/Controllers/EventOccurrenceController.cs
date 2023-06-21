using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Filters;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
///     Manage user event types with this crud controller
/// </summary>
[ApiController]
[Authorize]
[Route(template: "v1/event/occurrence")]
public class EventOccurrenceController: EventBaseController<OccurrenceEvent>
{
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EventOccurrenceController(IApiRequestAccessor         requestAccessor,
                                     IValidator<OccurrenceEvent> validator,
                                     IMediator                   mediator)
        : base(requestAccessor: requestAccessor, validator: validator, mediator: mediator) {
        _mediator = mediator;
    }

    /// <summary> Get occurrence events for a period </summary>
    [HttpGet]
    public async Task<ActionResult<EventsForPeriodResponse>> GetForPeriodAsync([FromQuery] ByPeriodRequest request,
                                                                               CancellationToken           cancellationToken) {
        var query       = new OccurrenceEventGetForPeriod(Start: request.Start, End: request.End, Context: OperationContext);
        var occurrences = await _mediator.Send(request: query, cancellationToken: cancellationToken);
        return new EventsForPeriodResponse(new GetEventsForPeriodRequest(Start: request.Start, End: request.End), Occurrences: occurrences, Durations: null);
    }

    /// <summary> Create an occurrence event </summary>
    [HttpPost]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<OccurrenceEvent>> CreateOccurrenceAsync(OccurrenceEventCreateRequest request,
                                                                           CancellationToken            cancellationToken) {
        await ValidateRequestAsync(request: request, cancellationToken: cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(modelStateDictionary: ModelState);
        var cqrsResult = await _mediator.Send(new OccurrenceEventCreateCommand(EventInfo: request, SaveChanges: true, Context: OperationContext),
                                              cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<OccurrenceEvent>> UpdateOccurrenceAsync(OccurrenceEventUpdateRequest request,
                                                                           CancellationToken            cancellationToken) {
        await ValidateRequestAsync(request: request, cancellationToken: cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(modelStateDictionary: ModelState);
        var cqrsResult = await _mediator.Send(new OccurrenceEventUpdateCommand(EventInfo: request, SaveChanges: true, Context: OperationContext),
                                              cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: cqrsResult);
    }
}