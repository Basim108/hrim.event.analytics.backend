using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Services;

namespace Hrim.Event.Analytics.Analysis.Services;

public class AnalysisSettingsFactory: IAnalysisSettingsFactory
{
    private static readonly List<AnalysisConfigByEventType> DefaultSettings;

    static AnalysisSettingsFactory() {
        DefaultSettings = new() {
            new AnalysisConfigByEventType {
                AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
                IsOn         = true
            },
            new() {
                AnalysisCode = FeatureCodes.GAP_ANALYSIS,
                IsOn         = true,
                Settings = new Dictionary<string, string> {
                    { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1.00:00:00" }
                }
            }
        };
    }

    /// <inheritdoc />
    public List<AnalysisConfigByEventType> GetDefaultSettings() => DefaultSettings;

    /// <inheritdoc />
    public List<AnalysisConfigByEventType>? GetMissedSettings(List<AnalysisConfigByEventType>? settings) {
        if (settings == null || settings.Count == 0)
            return DefaultSettings;
        if (settings.Count >= DefaultSettings.Count)
            return null;

        var missedSettings = new List<AnalysisConfigByEventType>(DefaultSettings.Count);
        foreach (var defSetting in DefaultSettings) {
            if (settings.Any(x => x.AnalysisCode == defSetting.AnalysisCode))
                continue;
            missedSettings.Add(defSetting);
        }
        return missedSettings;
    }
}