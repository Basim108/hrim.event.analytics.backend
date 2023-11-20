using Hrim.Event.Analytics.Abstractions.Entities.Analysis;

namespace Hrim.Event.Analytics.Abstractions.Services;

/// <summary> Creates analysis settings </summary>
public interface IAnalysisSettingsFactory
{
    /// <summary> Create a default settings </summary>
    List<AnalysisConfigByEventType> GetDefaultSettings();

    /// <summary> Check incoming list of settings and returns default settings for missed analysis</summary>
    /// <returns> Returns a list of missed settings, and null if no settings are missed</returns>
    List<AnalysisConfigByEventType>? GetMissedSettings(List<AnalysisConfigByEventType>? settings);
}