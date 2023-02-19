using Hrim.Event.Analytics.Abstractions.Entities.Account;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users;

/// <summary> Creates a hrim user instance </summary>
/// <param name="CorrelationId">
///     <see cref="BaseRequest" />
/// </param>
/// <param name="SaveChanges">If true, then changes will be flushed to the storages</param>
public record HrimUserCreateCommand(Guid CorrelationId, bool SaveChanges)
    : BaseRequest(CorrelationId), IRequest<HrimUser>;