using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Abstractions.Entities.Account;

/// <summary>
///     Entity present a user from a specific identity provider such as Google, Facebook, etc
/// </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ExternalUserProfile: HrimEntity<long>
{
    /// <summary>
    ///     An id of event analytics user
    /// </summary>
    public long HrimUserId { get; set; }

    /// <summary> Event analytics user that associated with this external user profile </summary>
    public virtual HrimUser? HrimUser { get; set; }

    /// <summary>
    ///     A user id in external identity provider
    /// </summary>
    public string ExternalUserId { get; set; } = null!;

    /// <summary>
    ///     Identity provider that provided this profile
    /// </summary>
    public ExternalIdp Idp { get; set; }

    /// <summary> A user email </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     If null then profile was linked but never used as a login
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary> </summary>
    public string? FullName { get; set; }

    /// <summary> </summary>
    public string? FirstName { get; set; }

    /// <summary> </summary>
    public string? LastName { get; set; }

    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(ExternalUserProfile another) {
        base.CopyTo(another: another);
        another.LastName       = LastName;
        another.FirstName      = FirstName;
        another.FullName       = FullName;
        another.LastLogin      = LastLogin;
        another.Email          = Email;
        another.Idp            = Idp;
        another.ExternalUserId = ExternalUserId;
        another.HrimUserId     = HrimUserId;
        another.HrimUser       = HrimUser;
    }
}