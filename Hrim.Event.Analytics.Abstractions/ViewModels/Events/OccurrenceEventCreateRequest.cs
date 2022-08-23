namespace Hrim.Event.Analytics.Abstractions.ViewModels.Events; 

/// <summary>
/// Model for occurrence event creation
/// </summary>
public class OccurrenceEventCreateRequest {
    /// <summary>
    /// Event type id on which current event is based.
    /// </summary>
    public Guid EventTypeId { get; set; }
    
    /// <summary>
    /// Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
}