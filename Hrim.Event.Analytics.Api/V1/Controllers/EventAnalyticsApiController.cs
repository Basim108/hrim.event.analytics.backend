using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                return StatusCode((int)HttpStatusCode.Gone);
            case CqrsResultCode.Conflict:
                if (string.IsNullOrWhiteSpace(cqrsResult.Info))
                    return StatusCode((int)HttpStatusCode.Conflict);
                return StatusCode((int)HttpStatusCode.Conflict, JsonConvert.SerializeObject(cqrsResult.Info));
            case CqrsResultCode.Forbidden:
                return StatusCode((int)HttpStatusCode.Forbidden, ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
            case CqrsResultCode.Locked:
                return StatusCode((int)HttpStatusCode.Locked);
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Created:
            case CqrsResultCode.Ok:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<TEntity?>(cqrsResult);
    }
}