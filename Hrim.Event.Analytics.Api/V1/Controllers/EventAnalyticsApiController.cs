using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Api.Services;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Base controller for all event analytics api controllers
/// </summary>
public class EventAnalyticsApiController: ControllerBase {
    private readonly IApiRequestAccessor _requestAccessor;

    /// <summary> </summary>
    public EventAnalyticsApiController(IApiRequestAccessor requestAccessor) {
        _requestAccessor = requestAccessor;
    }

    /// <summary>
    /// Context data about authorized user, correlation info, etc.
    /// </summary>
    public OperationContext OperationContext => _requestAccessor.GetOperationContext();

    /// <summary> Process CQRS result for update endpoints </summary>
    protected ActionResult<TEntity> ProcessCqrsResult<TEntity>(CqrsResult<TEntity?> cqrsResult) {
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new EmptyResult();
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.Forbidden:
                return Forbid(ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
            case CqrsResultCode.Locked:
                Response.StatusCode = (int)HttpStatusCode.Locked;
                return new EmptyResult();
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Created:
            case CqrsResultCode.Ok:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<TEntity?>(cqrsResult);
    }
}