using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary>
/// Describes a feature that might be on/off
/// </summary>
public class HrimFeature: HrimEntity<Guid>
{
    /// <summary>
    /// When a feature is off then its hangfire jobs, in case existed, should not be proceeded or scheduled.
    /// and in case feature represents an analysis (e.g. count, gap) this analysis should not appear in the list of available analysis.
    /// By default a feature is off
    /// </summary>
    public bool IsOn { get; set; }

    /// <summary> Type of the feature </summary>
    public FeatureType FeatureType { get; set; }
    
    /// <summary> Feature description </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Environment variable name that controls is this feature set on/off
    /// </summary>
    public string VariableName { get; set; } = string.Empty;

    /// <summary> Feature code </summary>
    public string Code { get; set; } = string.Empty;
}