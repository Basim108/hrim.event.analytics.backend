namespace Hrim.Event.Analytics.Abstractions.ViewModels.Events; 

/// <summary>
/// Model for occurrence event creation
/// </summary>
public class UpdateOccurrenceEventRequest {
    /// <summary> Event id </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Event type id on which current event is based.
    /// </summary>
    public Guid EventTypeId { get; set; }
    
    /// <summary>
    /// Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
    
    /// <summary>
    /// Update is possible only when this token equals to the token in the storage
    /// </summary>
    public long ConcurrentToken { get; set; }
}