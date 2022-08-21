namespace Hrim.Event.Analytics.Abstractions.Enums; 

/// <summary> Supported event types </summary>
public enum EventType {
    /// <summary>
    /// When the main importance is the fact that an event occurred.
    /// <br/>This kind of events may occur several times a day.
    /// </summary>
    Occurrence,
    /// <summary>
    /// When it is important to register an event that has start time and end time this system_event_type can be used.
    /// <br/>This kind of events may occur several times a day and can cross each other.
    /// </summary>
    Duration
}