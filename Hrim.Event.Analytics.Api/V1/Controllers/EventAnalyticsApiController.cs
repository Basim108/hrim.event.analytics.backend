using System.Net;
using FluentValidation;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
///     Base controller for all event analytics api controllers
/// </summary>
public class EventAnalyticsApiController<TEntity>: ControllerBase
{
    private readonly IApiRequestAccessor _requestAccessor;
    private readonly IValidator<TEntity> _validator;

    /// <summary> </summary>
    public EventAnalyticsApiController(IApiRequestAccessor requestAccessor,
                                       IValidator<TEntity> validator) {
        _requestAccessor = requestAccessor;
        _validator       = validator;
    }

    /// <summary>
    ///     Context data about authorized user, correlation info, etc.
    /// </summary>
    protected OperationContext OperationContext => _requestAccessor.GetOperationContext();

    /// <summary> Process CQRS result for update endpoints </summary>
    protected ActionResult<TEntity> ProcessCqrsResult(CqrsResult<TEntity?> cqrsResult) {
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                return StatusCode((int)HttpStatusCode.Gone);
            case CqrsResultCode.Conflict:
                if (string.IsNullOrWhiteSpace(value: cqrsResult.Info))
                    return StatusCode((int)HttpStatusCode.Conflict);
                return StatusCode((int)HttpStatusCode.Conflict, JsonConvert.SerializeObject(value: cqrsResult.Info));
            case CqrsResultCode.Forbidden:
                return StatusCode((int)HttpStatusCode.Forbidden, value: ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
            case CqrsResultCode.Locked:
                return StatusCode((int)HttpStatusCode.Locked);
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Created:
            case CqrsResultCode.Ok:
                return Ok(value: cqrsResult.Result);
        }

        throw new UnexpectedCqrsResultException<TEntity?>(cqrsResult: cqrsResult);
    }

    /// <summary>
    ///     Provide async validation for any kind of events
    /// </summary>
    protected async Task ValidateRequestAsync(TEntity request, CancellationToken cancellationToken) {
        var validationResult = await _validator.ValidateAsync(instance: request, cancellation: cancellationToken);
        if (validationResult.IsValid)
            return;
        foreach (var error in validationResult.Errors) ModelState.AddModelError(key: error.PropertyName, errorMessage: error.ErrorMessage);
    }
}