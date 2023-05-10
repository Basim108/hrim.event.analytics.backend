using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hrim.Event.Analytics.Api.Filters;

/// <summary>
///     Set created_by_id property from authorization context
/// </summary>
public class SetOwnerTypeFilterAttribute : TypeFilterAttribute
{
    /// <inheritdoc />
    public SetOwnerTypeFilterAttribute()
        : base(typeof(SetOwnerActionFilter))
    {
    }
}

/// <summary>
///     Set created_by_id property from authorization context
/// </summary>
internal sealed class SetOwnerActionFilter : IActionFilter, IAsyncActionFilter
{
    private readonly ILogger<SetOwnerActionFilter> _logger;
    private readonly IApiRequestAccessor _requestAccessor;

    public SetOwnerActionFilter(IApiRequestAccessor requestAccessor,
        ILogger<SetOwnerActionFilter> logger)
    {
        _requestAccessor = requestAccessor;
        _logger = logger;
    }

    private Guid _internalUserId;

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var isNoOwnerSet = true;
        foreach (var (name, value) in context.ActionArguments)
        {
            if (value is not IHasOwner entity)
                continue;
            entity.CreatedById = _internalUserId;
            isNoOwnerSet = false;
            _logger.LogDebug(ApiLogs.SET_OPERATOR_ID_TO_ENTITY, _internalUserId, name);
        }

        if (isNoOwnerSet) _logger.LogWarning(ApiLogs.NO_OWNER_SET_IN_FILTER);
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // as there is no way to create only OnActionExecuting filter, leave this method blanked.
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        _internalUserId = await _requestAccessor.GetInternalUserIdAsync(CancellationToken.None);
        OnActionExecuting(context);
        var resultContext = await next();
        OnActionExecuted(resultContext);
    }
}