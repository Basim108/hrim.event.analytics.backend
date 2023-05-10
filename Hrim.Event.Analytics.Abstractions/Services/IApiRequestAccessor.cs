using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs;

namespace Hrim.Event.Analytics.Abstractions.Services;

#pragma warning disable CS1591
public interface IApiRequestAccessor
{
    Guid GetCorrelationId();

    IEnumerable<Claim> GetUserClaims();

    OperationContext GetOperationContext();

    /// <summary> Get cached per request internal user id by operation context </summary>
    Task<Guid> GetInternalUserIdAsync(CancellationToken cancellation);
}