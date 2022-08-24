namespace Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events; 

/// <summary> </summary>
public class BaseEventUpdateRequest: EntityUpdateRequest {
    /// <summary>
    /// Event type id on which current event is based.
    /// </summary>
    public Guid EventTypeId { get; set; }
}