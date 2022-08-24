namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events; 

/// <summary>
/// Model for occurrence event creation
/// </summary>
public class OccurrenceEventCreateRequest: BaseEventCreateRequest {
    /// <summary>
    /// Date and time with end-user timezone when an event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; }
}