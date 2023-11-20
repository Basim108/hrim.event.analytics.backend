using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Middleware;
using MediatR;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Services;

public class ApiRequestAccessor: IApiRequestAccessor
{
    private readonly IHttpContextAccessor        _httpContextAccessor;
    private readonly ILogger<ApiRequestAccessor> _logger;
    private readonly IMediator                   _mediator;

    private long _internalUserId;

    public ApiRequestAccessor(IHttpContextAccessor        httpContextAccessor,
                              ILogger<ApiRequestAccessor> logger,
                              IMediator                   mediator) {
        _httpContextAccessor = httpContextAccessor;
        _logger              = logger;
        _mediator            = mediator;
    }

    public Guid GetCorrelationId() { return Guid.Parse(GetStringCorrelationId()); }

    public OperationContext GetOperationContext() { return new OperationContext(GetUserClaims(), GetCorrelationId()); }

    public IEnumerable<Claim> GetUserClaims() {
        var isNotAuthorized = !(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false);
        if (isNotAuthorized) {
            _logger.LogWarning("Getting user claims while user is not authorized");
        }
        return isNotAuthorized
                   ? Enumerable.Empty<Claim>()
                   : _httpContextAccessor.HttpContext!.User.Claims;
    }

    public async Task<long> GetInternalUserIdAsync(CancellationToken cancellation) {
        if (_internalUserId == default)
            _internalUserId = await _mediator.Send(new GetInternalUserIdQuery(GetOperationContext()), cancellationToken: cancellation);
        return _internalUserId;
    }

    private string GetStringCorrelationId() {
        var result = Guid.Empty.ToString();
        if (_httpContextAccessor.HttpContext == null)
            return result;
        var context = _httpContextAccessor.HttpContext;
        if (!context.Response.Headers.TryGetValue(key: CorrelationMiddleware.CORRELATION_ID_HEADER, out var value))
            return result;
        return string.IsNullOrWhiteSpace(value.ToString()) ? result : value.ToString();
    }
}