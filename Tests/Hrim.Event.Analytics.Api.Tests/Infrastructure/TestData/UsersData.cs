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

    public UsersData(EventAnalyticDbContext context) { _context = context; }

    public HrimUser EnsureUserExistence(Guid        id,
                                        bool        isDeleted  = false,
                                        string      externalId = UsersData.EXTERNAL_ID,
                                        ExternalIdp idp        = ExternalIdp.Facebook,
                                        string      email      = UsersData.EMAIL) {
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
        var externalProfile = new ExternalUserProfile() {
            ExternalUserId  = externalId,
            Email           = email,
            Idp             = idp,
            CreatedAt       = DateTime.UtcNow.TruncateToMilliseconds(),
            LastLogin       = DateTime.UtcNow.TruncateToMilliseconds(),
            ConcurrentToken = 1,
            HrimUser        = user,
            HrimUserId      = id
        };
        if (isDeleted)
            user.IsDeleted = true;
        _context.ExternalUserProfiles.Add(externalProfile);
        _context.HrimUsers.Add(user);
        _context.SaveChanges();
        return user;
    }

    /// <summary> Creates a user if not exists and create a profile for this user </summary>
    public ExternalUserProfile CreateUniqueLogin(Guid?       userId     = null,
                                                 string?     email      = null,
                                                 string?     externalId = null,
                                                 ExternalIdp idp        = ExternalIdp.Google,
                                                 bool        isDeleted  = false) {
        var user = EnsureUserExistence(userId ?? Guid.NewGuid());
        externalId ??= Guid.NewGuid().ToString();
        email      ??= $"{externalId}@mailinator.com";
        var profile = new ExternalUserProfile {
            Idp             = idp,
            Email           = email,
            ExternalUserId  = externalId,
            FullName        = FULL_NAME,
            FirstName       = FIRST_NAME,
            LastName        = LAST_NAME,
            HrimUserId      = user.Id,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            LastLogin       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.ExternalUserProfiles.Add(profile);
        if (isDeleted)
            user.IsDeleted = true;
        _context.SaveChanges();
        return profile;
    }
}