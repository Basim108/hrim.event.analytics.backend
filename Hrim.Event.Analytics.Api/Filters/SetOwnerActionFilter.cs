using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hrim.Event.Analytics.Api.Filters;

/// <summary>
/// Set created_by_id property from authorization context
/// </summary>
public class SetOwnerTypeFilterAttribute: TypeFilterAttribute {
    /// <inheritdoc />
    public SetOwnerTypeFilterAttribute()
        : base(typeof(SetOwnerActionFilter)) { }
}

/// <summary>
/// Set created_by_id property from authorization context
/// </summary>
internal sealed class SetOwnerActionFilter: IActionFilter {
    private readonly IApiRequestAccessor           _requestAccessor;
    private readonly ILogger<SetOwnerActionFilter> _logger;
        
    public SetOwnerActionFilter(IApiRequestAccessor           requestAccessor,
                                ILogger<SetOwnerActionFilter> logger) {
        _requestAccessor = requestAccessor;
        _logger          = logger;
    }

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context) {
        var operatorId = _requestAccessor.GetAuthorizedUserId();
        foreach (var (name, value) in context.ActionArguments) {
            if (value is not IHasOwner entity)
                continue;
            entity.CreatedById = operatorId;
            var message = string.Format(ApiLogs.SET_OPERATOR_ID_TO_ENTITY, name);
            _logger.LogDebug(message, operatorId);
        }
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context) {
        // as there is no way to create only OnActionExecuting filter, leave this method blanked.
    }
}