using System.Net;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Base controller for all event analytics api controllers
/// </summary>
public class EventAnalyticsApiController: ControllerBase {
    /// <summary> Process CQRS result for create endpoints </summary>
    protected ActionResult<TEntity> ProcessCreateResult<TEntity>(CqrsResult<TEntity?> cqrsResult) {
        if (cqrsResult == null)
            throw new ArgumentNullException(nameof(cqrsResult));

        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new ObjectResult(cqrsResult.Result);
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.Ok:
            case CqrsResultCode.Created:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<TEntity?>(cqrsResult);
    }

    /// <summary> Process CQRS result for update endpoints </summary>
    protected ActionResult<TEntity> ProcessUpdateResult<TEntity>(CqrsResult<TEntity?> cqrsResult) {
        switch (cqrsResult.StatusCode) {
            case CqrsResultCode.EntityIsDeleted:
                Response.StatusCode = (int)HttpStatusCode.Gone;
                return new EmptyResult();
            case CqrsResultCode.Conflict:
                return Conflict(cqrsResult.Result);
            case CqrsResultCode.NotFound:
                return NotFound();
            case CqrsResultCode.Ok:
                return Ok(cqrsResult.Result);
        }
        throw new UnexpectedCqrsResultException<TEntity?>(cqrsResult);
    }

    /// <summary> Process Get endpoint results </summary>
    protected ActionResult<TEntity> ProcessGetByIdResult<TEntity>(TEntity? entity) where TEntity : HrimEntity {
        if (entity == null)
            return NotFound();
        if (entity.IsDeleted == true) {
            Response.StatusCode = (int)HttpStatusCode.Gone;
            return new EmptyResult();
        }
        return Ok(entity);
    }
}