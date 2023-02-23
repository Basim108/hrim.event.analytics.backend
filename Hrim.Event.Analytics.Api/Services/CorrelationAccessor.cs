using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Api.Authentication;
using Hrim.Event.Analytics.Api.Middleware;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Services;

public interface IApiRequestAccessor
{
    Guid GetAuthorizedUserId();
    Guid GetCorrelationId();

    OperationContext GetOperationContext();
}

public class ApiRequestAccessor : IApiRequestAccessor
{
    private readonly ILogger<ApiRequestAccessor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiRequestAccessor(ILogger<ApiRequestAccessor> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetAuthorizedUserId()
    {
#if DEBUG
            return Guid.Parse("07f6a3ec-03b7-4738-b6a6-f94e5782b94b");
#endif
        var isNotAuthorized = !(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false);
        if (isNotAuthorized)
            return Guid.Empty;
        var userIdClaim = _httpContextAccessor.HttpContext!
            .User
            .Claims
            .FirstOrDefault(x => x.Type == HrimClaims.HRIM_USER_ID);
        var userId = userIdClaim == null
            ? Guid.Empty
            : Guid.Parse(userIdClaim.Value);
        _logger.LogDebug(ApiLogs.INTERNAL_USER_ID, userId);
        return userId;
    }

    public Guid GetCorrelationId()
    {
        return Guid.Parse(GetStringCorrelationId());
    }

    public OperationContext GetOperationContext() => new(GetAuthorizedUserId(), GetCorrelationId());

    private string GetStringCorrelationId()
    {
        var result = Guid.Empty.ToString();
        if (_httpContextAccessor.HttpContext == null)
            return result;
        var context = _httpContextAccessor.HttpContext;
        if (!context.Response.Headers.TryGetValue(CorrelationMiddleware.CORRELATION_ID_HEADER, out var value))
            return result;
        return string.IsNullOrWhiteSpace(value.ToString()) ? result : value.ToString();
    }
}