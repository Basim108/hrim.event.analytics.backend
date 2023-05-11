using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hrim.Event.Analytics.Api.Filters;

/// <summary>
///     Set created_by_id property from authorization context
/// </summary>
public class SetOwnerTypeFilterAttribute: TypeFilterAttribute
{
    /// <inheritdoc />
    public SetOwnerTypeFilterAttribute()
        : base(typeof(SetOwnerActionFilter)) { }
}

/// <summary>
///     Set created_by_id property from authorization context
/// </summary>
internal sealed class SetOwnerActionFilter: IActionFilter, IAsyncActionFilter
{
    private readonly ILogger<SetOwnerActionFilter> _logger;
    private readonly IApiRequestAccessor           _requestAccessor;

    private Guid _internalUserId;

    public SetOwnerActionFilter(IApiRequestAccessor           requestAccessor,
                                ILogger<SetOwnerActionFilter> logger) {
        _requestAccessor = requestAccessor;
        _logger          = logger;
    }

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context) {
        var isNoOwnerSet = true;
        foreach (var (name, value) in context.ActionArguments) {
            if (value is not IHasOwner entity)
                continue;
            entity.CreatedById = _internalUserId;
            isNoOwnerSet       = false;
            _logger.LogDebug(message: ApiLogs.SET_OPERATOR_ID_TO_ENTITY, _internalUserId, name);
        }

        if (isNoOwnerSet) _logger.LogWarning(message: ApiLogs.NO_OWNER_SET_IN_FILTER);
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context) {
        // as there is no way to create only OnActionExecuting filter, leave this method blanked.
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        _internalUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: CancellationToken.None);
        OnActionExecuting(context: context);
        var resultContext = await next();
        OnActionExecuted(context: resultContext);
    }
}