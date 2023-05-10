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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator            _mediator;

    public ApiRequestAccessor(IHttpContextAccessor httpContextAccessor,
                              IMediator            mediator) {
        _httpContextAccessor = httpContextAccessor;
        _mediator            = mediator;
    }

    public Guid GetCorrelationId() { return Guid.Parse(GetStringCorrelationId()); }

    public OperationContext GetOperationContext() => new(GetUserClaims(), GetCorrelationId());

    public IEnumerable<Claim> GetUserClaims() {
        var isNotAuthorized = !(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false);
        return isNotAuthorized
                   ? Enumerable.Empty<Claim>()
                   : _httpContextAccessor.HttpContext!.User.Claims;
    }

    private Guid _internalUserId;

    /// <inheritdoc />
    public async Task<Guid> GetInternalUserIdAsync(CancellationToken cancellation) {
        if (_internalUserId == Guid.Empty) {
            _internalUserId = await _mediator.Send(new GetInternalUserIdQuery(GetOperationContext()), cancellation);
        }
        return _internalUserId;
    }

    private string GetStringCorrelationId() {
        var result = Guid.Empty.ToString();
        if (_httpContextAccessor.HttpContext == null)
            return result;
        var context = _httpContextAccessor.HttpContext;
        if (!context.Response.Headers.TryGetValue(CorrelationMiddleware.CORRELATION_ID_HEADER, out var value))
            return result;
        return string.IsNullOrWhiteSpace(value.ToString()) ? result : value.ToString();
    }
}