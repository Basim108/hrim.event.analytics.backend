#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS8618

namespace Hrim.Event.Analytics.Abstractions.Entities.Analysis;

/// <summary>
/// Configuration of an analysis that will be made around events of a particular event-type
/// </summary>
public class AnalysisConfigByEventType
{
    /// <summary> Events of this event type id will be analysed </summary>
    public long EventTypeId { get; set; }
    
    public string AnalysisCode { get; set; }

    /// <summary>
    /// Enable/disable analysis for a particular event-type
    /// By default is On
    /// </summary>
    public bool IsOn { get; set; }

    /// <summary> Json of an analysis settings </summary>
    public IDictionary<string, string>? Settings { get; set; }

    /// <summary> Date and UTC time of entity instance creation </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary> Date and UTC time of entity instance last update </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary> Update is possible only when this token equals to the token in the storage </summary>
    public long ConcurrentToken { get; set; }
}