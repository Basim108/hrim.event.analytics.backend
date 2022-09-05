using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

#if RELEASE
using Microsoft.AspNetCore.Authorization;
#endif

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Manage user event types with this crud controller
/// </summary>
[ApiController]
#if RELEASE
[Authorize]
#endif
[Route("v1/event/occurrence")]
public class EventOccurrenceController: EventBaseController<OccurrenceEvent> {
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EventOccurrenceController(IApiRequestAccessor         requestAccessor,
                                     IValidator<OccurrenceEvent> validator,
                                     IMediator                   mediator)
        : base(requestAccessor, validator, mediator) {
        _mediator = mediator;
    }

    /// <summary> Create an occurrence event </summary>
    [HttpPost]
    public async Task<ActionResult<OccurrenceEvent>> CreateOccurrenceAsync(OccurrenceEventCreateRequest request, CancellationToken cancellationToken) {
        await ValidateRequestAsync(request, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var cqrsResult = await _mediator.Send(new OccurrenceEventCreateCommand(request, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut]
    public async Task<ActionResult<OccurrenceEvent>> UpdateOccurrenceAsync(OccurrenceEventUpdateRequest request, CancellationToken cancellationToken) {
        await ValidateRequestAsync(request, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var cqrsResult = await _mediator.Send(new OccurrenceEventUpdateCommand(request, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }
}