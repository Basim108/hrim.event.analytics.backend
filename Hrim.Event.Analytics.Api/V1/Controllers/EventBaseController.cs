using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Controller for all types of user events
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public class EventBaseController<TEvent>: EventAnalyticsApiController<TEvent>
    where TEvent : BaseEvent, new() {
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EventBaseController(IApiRequestAccessor requestAccessor,
                               IValidator<TEvent>  validator,
                               IMediator           mediator): base(requestAccessor, validator) {
        _mediator = mediator;
    }

    /// <summary> Get user event by id </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TEvent>> GetEventByIdAsync([FromRoute] ByIdRequest request, CancellationToken cancellationToken) {
        var occurrenceResult = await _mediator.Send(new GetEventById<TEvent>(request.Id, IsNotTrackable: true, OperationContext),
                                                    cancellationToken);
        return ProcessCqrsResult(occurrenceResult);
    }
}