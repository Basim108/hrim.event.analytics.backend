namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary> Authorized user </summary>
public class HrimUser: HrimEntity {
    /// <summary> A user email </summary>
    public string? Email { get; set; }
}