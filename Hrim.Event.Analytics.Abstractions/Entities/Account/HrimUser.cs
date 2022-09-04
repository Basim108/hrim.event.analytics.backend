using Hrim.Event.Analytics.Abstractions.Extensions;

namespace Hrim.Event.Analytics.Abstractions.Entities.Account;

/// <summary> Authorized user </summary>
public class HrimUser: HrimEntity {
    /// <summary>
    /// All external identity provider profiles linked to this user
    /// </summary>
    public IList<ExternalUserProfile> ExternalProfiles { get; set; } = null!;
    
    /// <summary> copy all entity properties to the another entity </summary>
    public void CopyTo(HrimUser another) {
        base.CopyTo(another);
        another.ExternalProfiles = ExternalProfiles.CopyListTo();
    }
}