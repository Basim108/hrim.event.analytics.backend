#pragma warning disable CS8618
namespace Hrim.Event.Analytics.Abstractions.Entities.Analysis;

/// <summary>
/// Provides a result of analysis calculation for a specific entity
/// </summary>
public class StatisticsForEntity
{
    /// <summary>
    /// Id of an entity e.g. occurrence/duration events or event-types for which theis calculation was made. 
    /// </summary>
    public long? EntityId { get; set; }
    
    /// <summary> Code of analysis <see cref="FeatureCodes"/> </summary>
    public string AnalysisCode { get; set; }

    /// <summary> Result of calculation serialized to JSON </summary>
    public string? ResultJson { get; set; }

    /// <summary> Date and UTC time when an analysis has been started </summary>
    public DateTime StartedAt { get; set; }

    /// <summary> Date and UTC time when an analysis has been finished. </summary>
    public DateTime FinishedAt { get; set; }

    /// <summary> The last run correlation id </summary>
    public Guid CorrelationId { get; set; }
}