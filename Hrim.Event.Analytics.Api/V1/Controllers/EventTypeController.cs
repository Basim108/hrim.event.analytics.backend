using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
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
[Route(template: "v1/event-type")]
public class EventTypeController: EventAnalyticsApiController<UserEventType>
{
    private readonly IValidator<UserEventType> _validator;
    private readonly IMediator                 _mediator;

    /// <summary> </summary>
    public EventTypeController(IApiRequestAccessor       requestAccessor,
                               IValidator<UserEventType> validator,
                               IMediator                 mediator)
        : base(requestAccessor) {
        _validator = validator;
        _mediator  = mediator;
    }

    /// <summary> Get all user event types </summary>
    [HttpGet]
    public Task<IList<ViewEventType>> GetAllAsync(CancellationToken cancellationToken) {
        return _mediator.Send(new EventTypeGetAllMine(Context: OperationContext, IncludeOthersPublic: true), cancellationToken: cancellationToken);
    }

    /// <summary> Get user event type by id </summary>
    [HttpGet(template: "{id}")]
    public async Task<ActionResult<UserEventType>> GetByIdAsync([FromRoute] ByIdRequest request,
                                                                CancellationToken       cancellationToken) {
        var result = await _mediator.Send(new EventTypeGetById(Id: request.Id, IsNotTrackable: true, Context: OperationContext),
                                          cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: result);
    }

    /// <summary> Create a new event type </summary>
    [HttpPost]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<UserEventType>> CreateAsync(CreateEventTypeRequest request,
                                                               CancellationToken      cancellationToken) {
        var validationResult = await _validator.ValidateAsync(instance: request, cancellation: cancellationToken);
        ValidateRequest(validationResult, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(modelStateDictionary: ModelState);
        var cqrsResult = await _mediator.Send(new EventTypeCreateCommand(EventType: request, SaveChanges: true, Context: OperationContext),
                                              cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: cqrsResult);
    }

    /// <summary> Update an event type </summary>
    [HttpPut]
    [SetOwnerTypeFilter]
    public async Task<ActionResult<UserEventType>> UpdateAsync(UpdateEventTypeRequest request,
                                                               CancellationToken      cancellationToken) {
        var validationResult = await _validator.ValidateAsync(instance: request, cancellation: cancellationToken);
        ValidateRequest(validationResult, cancellationToken);
        if (!ModelState.IsValid)
            return ValidationProblem(modelStateDictionary: ModelState);
        var cqrsResult = await _mediator.Send(new EventTypeUpdateCommand(EventType: request, SaveChanges: true, Context: OperationContext),
                                              cancellationToken: cancellationToken);
        return ProcessCqrsResult(cqrsResult: cqrsResult);
    }
}