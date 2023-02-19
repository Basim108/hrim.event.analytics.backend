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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiRequestAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetAuthorizedUserId()
    {
        var isNotAuthorized = !(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false);
        if (isNotAuthorized)
            return Guid.Empty;
        var userIdClaim = _httpContextAccessor.HttpContext!
            .User
            .Claims
            .FirstOrDefault(x => x.Type == HrimClaims.HRIM_USER_ID);
        return userIdClaim == null
            ? Guid.Empty
            : Guid.Parse(userIdClaim.Value);
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