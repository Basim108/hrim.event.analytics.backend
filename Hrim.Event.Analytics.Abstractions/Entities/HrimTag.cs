using Hrim.Event.Analytics.Abstractions.Entities.Account;

namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary> A tag that could be linked to an instance of any entity</summary>
public class HrimTag: HrimEntity {
    /// <summary>A tag</summary>
    public string Tag { get; set; } = null!;

    /// <summary> A user id who created a tag </summary>
    public Guid CreatedById { get; set; }

    /// <summary> A user who created a tag </summary>
    public HrimUser? CreatedBy { get; set; }
}