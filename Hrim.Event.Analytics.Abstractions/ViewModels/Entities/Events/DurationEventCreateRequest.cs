namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events; 

/// <summary>
/// Model for occurrence event creation
/// </summary>
public class DurationEventCreateRequest: BaseEventCreateRequest {
    /// <summary>
    /// Date and time with end-user timezone when an event starts
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }
    
    /// <summary>
    /// Date and time with end-user timezone when an event finishes
    /// </summary>
    public DateTimeOffset? FinishedAt { get; set; }
}