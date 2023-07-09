using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Filters;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
///     Manage user event types with this crud controller
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route(template: "v1/event/duration")]
public class EventDurationController: EventBaseController<DurationEvent>
{
    private readonly IValidator<DurationEvent> _validator;
    private readonly IMediator                 _mediator;

    /// <summary> </summary>
    public EventDurationController(IApiRequestAccessor       requestAccessor,
                                   IValidator<DurationEvent> validator,
                                   IMediator                 mediator)
        : base(requestAccessor, mediator) {
        _validator = validator;
        _mediator  = mediator;
    }

    /// <summary> Get duration events for a period </summary>
    [HttpGet]
    public async Task<ActionResult<EventsForPeriodResponse>> GetForPeriodAsync([FromQuery] ByPeriodRequest request,
                                                                               CancellationToken           cancellationToken) {
        var query     = new DurationEventGetForPeriod(Start: request.Start, End: request.End, Context: OperationContext);
        var durations = await _mediator.Send(request: query, cancellationToken: cancellationToken);
        return new EventsForPeriodResponse(new GetEventsForPeriodRequest(Start: request.Start, End: request.End), Occurrences: null, Durations: durations);
    }

    /// <summary> Create a duration event </summary>
    [HttpPost]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<DurationEvent>> CreateDurationAsync(DurationEventCreateRequest request,
                                                                       CancellationToken          cancellationToken) {
        var validationResult = await _validator.ValidateAsync(instance: request, cancellation: cancellationToken);
        ValidateRequest(validationResult, cancellationToken: cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(modelStateDictionary: ModelState);
        var cqrsResult = await _mediator.Send(new DurationEventCreateCommand(EventInfo: request, SaveChanges: true, Context: OperationContext),
                                              cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: cqrsResult);
    }

    /// <summary> Update a duration event </summary>
    [HttpPut]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<DurationEvent>> UpdateDurationAsync(DurationEventUpdateRequest request,
                                                                       CancellationToken          cancellationToken) {
        var validationResult = await _validator.ValidateAsync(instance: request, cancellation: cancellationToken);
        ValidateRequest(validationResult, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(modelStateDictionary: ModelState);
        var cqrsResult = await _mediator.Send(new DurationEventUpdateCommand(EventInfo: request, SaveChanges: true, Context: OperationContext),
                                              cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: cqrsResult);
    }
}