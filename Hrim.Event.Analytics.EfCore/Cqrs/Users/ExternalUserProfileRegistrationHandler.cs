using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Users;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class ExternalUserProfileRegistrationHandler: IRequestHandler<ExternalUserProfileRegistration, ExternalUserProfile>
{
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

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<ExternalUserProfile> HandleAsync(ExternalUserProfileRegistration request,
                                                        CancellationToken               cancellationToken) {
        var externalId = request.Context.ExternalId();
        var email      = request.Context.Email;

        IQueryable<ExternalUserProfile> query = _context.ExternalUserProfiles
                                                        .Include(x => x.HrimUser);
        query = string.IsNullOrWhiteSpace(value: email)
                    ? query.Where(x => x.ExternalUserId == externalId)
                    : query.Where(x => x.ExternalUserId == externalId || x.Email == email);

        var existedList = await query.ToListAsync(cancellationToken: cancellationToken);

        ExternalUserProfile? result              = null;
        var                  shouldCreateProfile = existedList.Count == 0;
        foreach (var existedProfile in existedList) {
            var hasSameIdp        = existedProfile.Idp            == request.Context.Idp();
            var hasSameExternalId = existedProfile.ExternalUserId == externalId;
            var hasSameEmail      = existedProfile.Email          == email;
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
        // create inner user without saving
        var user = result?.HrimUser
                ?? await _mediator.Send(new HrimUserCreateCommand(CorrelationId: request.Context.CorrelationId,
                                                                  SaveChanges: false),
                                        cancellationToken: cancellationToken);
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        if (shouldCreateProfile) {
            result = new ExternalUserProfile {
                HrimUser        = user,
                ExternalUserId  = externalId,
                Email           = email,
                FullName        = request.Profile.FullName,
                FirstName       = request.Profile.FirstName,
                LastName        = request.Profile.LastName,
                Idp             = request.Context.Idp(),
                LastLogin       = now,
                CreatedAt       = now,
                UpdatedAt       = now,
                ConcurrentToken = 1
            };
            _context.ExternalUserProfiles.Add(entity: result);
        }
        else {
            result!.Email    = email;
            result.UpdatedAt = now;
            result.LastLogin = now;
            result.ConcurrentToken++;
        }
        await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        return result;
    }
}