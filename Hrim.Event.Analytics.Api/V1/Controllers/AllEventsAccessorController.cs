using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#if RELEASE
using Microsoft.AspNetCore.Authorization;
#endif

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Get access to events of all types at once </summary>
[ApiController]
#if RELEASE
[Authorize]
#endif
[Route("v1/event")]
public class AllEventsAccessorController: ControllerBase {
    private readonly IApiRequestAccessor _requestAccessor;
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public AllEventsAccessorController(IApiRequestAccessor requestAccessor, IMediator mediator) {
        _requestAccessor = requestAccessor;
        _mediator        = mediator;
    }

    /// <summary> Get user's events for a period </summary>
    [HttpGet]
    public async Task<EventsForPeriodResponse> GetUserEventsAsync([FromQuery]ByPeriodRequest request, CancellationToken cancellationToken) {
        var operationContext = _requestAccessor.GetOperationContext();
        var occurrences = await _mediator.Send(new OccurrenceEventGetForPeriod(request.Start, request.End, operationContext),
                                               cancellationToken);
        var durations = await _mediator.Send(new DurationEventGetForPeriod(request.Start, request.End, operationContext),
                                             cancellationToken);
        return new EventsForPeriodResponse(new GetEventsForPeriodRequest(request.Start, request.End), occurrences, durations);
    }
}