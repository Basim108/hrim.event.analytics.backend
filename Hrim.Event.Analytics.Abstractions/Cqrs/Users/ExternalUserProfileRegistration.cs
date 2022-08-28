using Hrim.Event.Analytics.Abstractions.Entities.Account;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users;

/// <summary>Registers an external user</summary>
/// <returns>a profile that was registered with a created or linked hrim user</returns>
public record ExternalUserProfileRegistration(Guid CorrelationId, ExternalUserProfile Profile)
    : BaseRequest(CorrelationId), IRequest<ExternalUserProfile>;