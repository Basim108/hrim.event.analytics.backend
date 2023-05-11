using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Get access to events of all types at once </summary>
[ApiController]
[Authorize]
[Route(template: "v1/event")]
public class AllEventsAccessorController: ControllerBase
{
    private readonly IMediator           _mediator;
    private readonly IApiRequestAccessor _requestAccessor;

    /// <summary> </summary>
    public AllEventsAccessorController(IApiRequestAccessor requestAccessor, IMediator mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Get user's events for a period </summary>
    [HttpGet]
    public async Task<EventsForPeriodResponse> GetUserEventsAsync([FromQuery] ByPeriodRequest request,
                                                                  CancellationToken           cancellationToken) {
        var operationContext = _requestAccessor.GetOperationContext();
        var occurrences = await _mediator.Send(
                                               new OccurrenceEventGetForPeriod(Start: request.Start, End: request.End, Context: operationContext),
                                               cancellationToken: cancellationToken);
        var durations = await _mediator.Send(
                                             new DurationEventGetForPeriod(Start: request.Start, End: request.End, Context: operationContext),
                                             cancellationToken: cancellationToken);
        return new EventsForPeriodResponse(new GetEventsForPeriodRequest(Start: request.Start, End: request.End),
                                           Occurrences: occurrences,
                                           Durations: durations);
    }
}