using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrimsoft.Core.Extensions;
using MediatR;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Users;

public class ExternalUserProfileBuildHandler: IRequestHandler<ExternalUserProfileBuild, ExternalUserProfile> {
    public Task<ExternalUserProfile> Handle(ExternalUserProfileBuild request, CancellationToken cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.Claims.IsNullOrEmpty())
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Claims)}");
        var profile = new ExternalUserProfile {
            Idp       = request.Idp,
            LastLogin = DateTime.UtcNow.TruncateToMicroseconds()
        };
        foreach (var (type, value) in request.Claims) {
            switch (type) {
                case ClaimTypes.NameIdentifier:
                    profile.ExternalUserId = value.Trim();
                    break;
                case ClaimTypes.Email:
                    profile.Email = value.Trim();
                    break;
                case ClaimTypes.Name:
                    profile.FullName = value.Trim();
                    break;
                case ClaimTypes.GivenName:
                    profile.FirstName = value.Trim();
                    break;
                case ClaimTypes.Surname:
                    profile.LastName = value.Trim();
                    break;
            }
        }
        if (string.IsNullOrWhiteSpace(profile.ExternalUserId))
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Claims)}[{nameof(ClaimTypes.NameIdentifier)}]");
        if (string.IsNullOrWhiteSpace(profile.Email))
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Claims)}[{nameof(ClaimTypes.Email)}]");
        return Task.FromResult(profile);
    }
}