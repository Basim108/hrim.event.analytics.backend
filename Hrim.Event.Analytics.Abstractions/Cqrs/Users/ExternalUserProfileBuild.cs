using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Enums;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Users; 

/// <summary>
/// Maps user claim to an external user profile instance
/// </summary>
/// <param name="CorrelationId"><see cref="BaseRequest"/></param>
/// <param name="Claims">User claims from the authorization context</param>
/// <param name="Idp">For which idp profile should be build</param>
public record ExternalUserProfileBuild(Guid CorrelationId, IDictionary<string, string> Claims, ExternalIdp Idp)
    :BaseRequest(CorrelationId), IRequest<ExternalUserProfile>;