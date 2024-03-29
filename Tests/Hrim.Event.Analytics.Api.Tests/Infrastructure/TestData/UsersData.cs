using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public class UsersData
{
    public const     string                 EXTERNAL_ID = "225583379887912482138";
    public const     string                 EMAIL       = "test@mailinator.com";
    public const     string                 FULL_NAME   = "Alan Turing";
    public const     string                 FIRST_NAME  = "Alan";
    public const     string                 LAST_NAME   = "Turing";
    private readonly EventAnalyticDbContext _context;
    private static   long                   _lastCreatedUserId;

    public UsersData(EventAnalyticDbContext context) { _context = context; }

    public HrimUser EnsureUserExistence(long        id,
                                        bool        isDeleted  = false,
                                        string      externalId = EXTERNAL_ID,
                                        ExternalIdp idp        = ExternalIdp.Facebook,
                                        string?     email      = EMAIL) {
        var existed = _context.HrimUsers.FirstOrDefault(x => x.Id == id);
        if (existed != null) {
            if (existed.IsDeleted != isDeleted) {
                existed.IsDeleted = isDeleted;
                _context.SaveChanges();
            }

            return existed;
        }

        var user = new HrimUser {
            Id              = id,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var externalProfile = new ExternalUserProfile {
            ExternalUserId  = externalId,
            Email           = email,
            Idp             = idp,
            CreatedAt       = now,
            UpdatedAt       = now,
            LastLogin       = now,
            ConcurrentToken = 1,
            HrimUser        = user,
            HrimUserId      = id
        };
        if (isDeleted)
            user.IsDeleted = true;
        _context.ExternalUserProfiles.Add(entity: externalProfile);
        _context.HrimUsers.Add(entity: user);
        _context.SaveChanges();
        return user;
    }

    /// <summary> Creates a user if not exists and create a profile for this user </summary>
    public ExternalUserProfile CreateUniqueLogin(long?       userId     = null,
                                                 string?     email      = null,
                                                 string?     externalId = null,
                                                 ExternalIdp idp        = ExternalIdp.Google,
                                                 bool        isDeleted  = false) {
        var user = EnsureUserExistence(userId ?? ++_lastCreatedUserId);
        externalId ??= Guid.NewGuid().ToString();
        email      ??= $"{externalId}@mailinator.com";
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var profile = new ExternalUserProfile {
            Idp             = idp,
            Email           = email,
            ExternalUserId  = externalId,
            FullName        = FULL_NAME,
            FirstName       = FIRST_NAME,
            LastName        = LAST_NAME,
            HrimUserId      = user.Id,
            CreatedAt       = now,
            UpdatedAt       = now,
            LastLogin       = now,
            ConcurrentToken = 1
        };
        _context.ExternalUserProfiles.Add(entity: profile);
        if (isDeleted)
            user.IsDeleted = true;
        _context.SaveChanges();
        return profile;
    }
}