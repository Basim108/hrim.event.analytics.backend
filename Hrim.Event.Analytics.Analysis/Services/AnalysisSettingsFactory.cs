using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Services;

namespace Hrim.Event.Analytics.Analysis.Services;

public class AnalysisSettingsFactory: IAnalysisSettingsFactory
{
    /// <inheritdoc />
    public List<AnalysisByEventType> GetDefaultSettings() => new() {
        new (){ AnalysisCode = FeatureCodes.COUNT_ANALYSIS, IsOn = true },
        new () {
            AnalysisCode = FeatureCodes.GAP_ANALYSIS, 
            IsOn         = true,
            Settings = new Dictionary<string, string>(){ 
                {AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "24:00:00"}
            }
        },
    };
}