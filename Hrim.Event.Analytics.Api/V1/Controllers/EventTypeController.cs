using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Filters;
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
[Route("v1/event-type")]
public class EventTypeController: EventAnalyticsApiController<UserEventType> {
    private readonly IMediator _mediator;

    /// <summary> </summary>
    public EventTypeController(IApiRequestAccessor       requestAccessor,
                               IValidator<UserEventType> validator,
                               IMediator                 mediator): base(requestAccessor, validator) {
        _mediator = mediator;
    }

    /// <summary> Get all user event types </summary>
    [HttpGet]
    public Task<IList<ViewEventType>> GetAllAsync(CancellationToken cancellationToken)
        => _mediator.Send(new EventTypeGetAllMine(OperationContext), cancellationToken);

    /// <summary> Get user event type by id </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserEventType>> GetByIdAsync([FromRoute] ByIdRequest request, CancellationToken cancellationToken) {
        var result = await _mediator.Send(new EventTypeGetById(request.Id, IsNotTrackable: true, OperationContext),
                                          cancellationToken);
        return ProcessCqrsResult(result);
    }

    /// <summary> Create a new event type </summary>
    [HttpPost]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<UserEventType>> CreateAsync(CreateEventTypeRequest request, CancellationToken cancellationToken) {
        await ValidateRequestAsync(request, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var cqrsResult = await _mediator.Send(new EventTypeCreateCommand(request, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }

    /// <summary> Update an event type </summary>
    [HttpPut]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<UserEventType>> UpdateAsync(UpdateEventTypeRequest request, CancellationToken cancellationToken) {
        await ValidateRequestAsync(request, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var cqrsResult = await _mediator.Send(new EventTypeUpdateCommand(request, SaveChanges: true, OperationContext),
                                              cancellationToken);
        return ProcessCqrsResult(cqrsResult);
    }
}