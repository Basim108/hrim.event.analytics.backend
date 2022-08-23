#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Enums; 

/// <summary>
/// Operations that are require a specific permission
/// </summary>
public enum HrimOperations {
    /// <summary>
    /// To avoid uninitialized, default operation value
    /// </summary>
    NotInitialized,
    Read,
    Create,
    Update,
    Delete,
    SoftDelete,
    Restore
}