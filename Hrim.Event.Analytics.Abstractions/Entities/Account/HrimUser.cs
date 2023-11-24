using Hrim.Event.Analytics.Abstractions.Extensions;

namespace Hrim.Event.Analytics.Abstractions.Entities.Account;

/// <summary> Authorized user </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class HrimUser: HrimEntity<long>
{
    /// <summary>
    ///     All external identity provider profiles linked to this user
    /// </summary>
    public virtual IList<ExternalUserProfile> ExternalProfiles { get; set; } = null!;

    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(HrimUser another) {
        base.CopyTo(another: another);
        another.ExternalProfiles = ExternalProfiles.CopyListTo<ExternalUserProfile, long>();
    }
}