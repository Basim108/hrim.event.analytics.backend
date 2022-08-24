namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities; 

/// <summary>
/// Each entity update requests shares these properties
/// </summary>
public class EntityUpdateRequest {
    /// <summary> Entity id </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Update is possible only when this token equals to the token in the storage
    /// </summary>
    public long ConcurrentToken { get; set; }
}