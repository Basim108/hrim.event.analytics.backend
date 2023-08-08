using Hrim.Event.Analytics.Abstractions.Entities.Analysis;

namespace Hrim.Event.Analytics.Abstractions.Services;

/// <summary> Creates analysis settings </summary>
public interface IAnalysisSettingsFactory
{
    /// <summary> Create a default settings </summary>
    List<AnalysisByEventType> GetDefaultSettings();
}