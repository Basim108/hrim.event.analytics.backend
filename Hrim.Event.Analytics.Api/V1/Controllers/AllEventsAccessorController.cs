using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Get access to events of all types at once </summary>
[ApiController]
[Route("v1/event")]
public class AllEventsAccessorController: EventAnalyticsApiController {
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public AllEventsAccessorController(IApiRequestAccessor requestAccessor,
                           IMediator           mediator): base(requestAccessor) {
        _mediator = mediator;
    }

    /// <summary> Get user's events for a period </summary>
    [HttpGet]
    public async Task<EventsForPeriodResponse> GetUserEventsAsync(DateOnly start, DateOnly end, CancellationToken cancellationToken) {
        var occurrences = await _mediator.Send(new GetUserOccurrencesForPeriod(start, end, OperationContext),
                                               cancellationToken);
        var durations = await _mediator.Send(new GetUserDurationsForPeriod(start, end, OperationContext),
                                             cancellationToken);
        return new EventsForPeriodResponse(new GetEventsForPeriodRequest(start, end), occurrences, durations);
    }
}