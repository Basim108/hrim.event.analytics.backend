using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users;

/// <summary>Registers an external user and link to an existed user by email, external_id, etc</summary>
/// <param name="Context">
///     <see cref="OperationRequest" />
/// </param>
/// <param name="Profile">profile to create</param>
/// <returns>a profile that was registered with a created or linked hrim user</returns>
public record ExternalUserProfileRegistration(OperationContext Context, UserProfileModel Profile)
    : OperationRequest(Context), IRequest<ExternalUserProfile>;