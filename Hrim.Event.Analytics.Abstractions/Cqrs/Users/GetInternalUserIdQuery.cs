using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users;

/// <summary>
///     Gets an internal user id by operational claims (external_id or email)
/// </summary>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
public record GetInternalUserIdQuery(OperationContext Context): OperationRequest(Context: Context), IRequest<long>;