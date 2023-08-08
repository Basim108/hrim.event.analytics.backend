using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Newtonsoft.Json;

#pragma warning disable CS8618

namespace Hrim.Event.Analytics.Abstractions.Entities.Analysis;

/// <summary>
/// Analysis that is made around events of a particular event-type
/// </summary>
public class AnalysisByEventType
{
    [JsonIgnore]
    public UserEventType? EventType { get; set; }

    /// <summary> Events of this event type id will be analysed </summary>
    public Guid EventTypeId { get; set; }

    /// <summary> Code of analysis </summary>
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