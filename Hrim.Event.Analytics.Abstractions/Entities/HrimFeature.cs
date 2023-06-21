namespace Hrim.Event.Analytics.Abstractions.Entities;

/// <summary>
/// Describes a feature that might be on/off
/// </summary>
public class HrimFeature: HrimEntity
{
    /// <summary>
    /// When a feature is off then its hangfire jobs, in case existed, should not be proceeded or scheduled.
    /// and in case feature represents an analysis (e.g. count, gap) this analysis should not appear in the list of available analysis.
    /// By default a feature is off
    /// </summary>
    public bool IsOn { get; set; }
}