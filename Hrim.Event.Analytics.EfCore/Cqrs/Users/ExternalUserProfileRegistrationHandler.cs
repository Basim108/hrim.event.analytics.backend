using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Users;

public class ExternalUserProfileRegistrationHandler: IRequestHandler<ExternalUserProfileRegistration, ExternalUserProfile> {
    private readonly EventAnalyticDbContext _context;
    private readonly IMediator              _mediator;

    public ExternalUserProfileRegistrationHandler(EventAnalyticDbContext context,
                                                  IMediator              mediator) {
        _context  = context;
        _mediator = mediator;
    }

    public Task<ExternalUserProfile> Handle(ExternalUserProfileRegistration request, CancellationToken cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.Profile == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Profile)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<ExternalUserProfile> HandleAsync(ExternalUserProfileRegistration request,
                                                        CancellationToken               cancellationToken) {
        var profile = request.Profile;
        var existedList = await _context.ExternalUserProfiles
                                        .Include(x => x.HrimUser)
                                        .Where(x => x.ExternalUserId == profile.ExternalUserId ||
                                                    x.Email          == profile.Email)
                                        .ToListAsync(cancellationToken);
        ExternalUserProfile? result              = null;
        var                  shouldCreateProfile = existedList.Count == 0;
        foreach (var existedProfile in existedList) {
            var hasSameIdp        = existedProfile.Idp            == profile.Idp;
            var hasSameExternalId = existedProfile.ExternalUserId == profile.ExternalUserId;
            var hasSameEmail      = existedProfile.Email          == profile.Email;
            if (hasSameExternalId && hasSameIdp) {
                result = existedProfile;
                break;
            }
            if (hasSameEmail) {
                result = existedProfile;
                if (hasSameIdp) {
                    shouldCreateProfile = false;
                    break;
                }
                shouldCreateProfile = true;
            }
        }
        var user = result?.HrimUser;
        if (user == null) {
            // create user without saving
            user = await _mediator.Send(new HrimUserCreateCommand(request.CorrelationId, SaveChanges: false),
                                        cancellationToken);
        }
        if (shouldCreateProfile) {
            profile.HrimUser        = user;
            profile.LastLogin       = DateTime.UtcNow.TruncateToMicroseconds();
            profile.CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds();
            profile.ConcurrentToken = 1;
            _context.ExternalUserProfiles.Add(profile);
            result = profile;
        }
        else {
            result!.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
            result.LastLogin  = DateTime.UtcNow.TruncateToMicroseconds();
            result.ConcurrentToken++;
        }
        await _context.SaveChangesAsync(cancellationToken);
        return result;
    }
}