using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;

public class GapSettings
{
    /// <summary>
    /// A length from which we start considering the gap.
    /// For example, when 1d it’s not a gap, and only starting from 2 days we consider events of a particular event-type as a gap.
    /// Therefore we setup this property to 2d.
    /// </summary>
    public TimeSpan MinimalGap { get; private set; }

    public GapSettings() { }

    public GapSettings(IDictionary<string, string> settings) {
        FromDictionary(settings);
    }
    
    public GapSettings(TimeSpan minimalGap) {
        MinimalGap = minimalGap;
    }
    
    /// <summary>
    /// Initialises class properties from dictionary
    /// </summary>
    /// <param name="settings">Dictionary from the storage</param>
    public void FromDictionary(IDictionary<string, string> settings) {
        if (settings.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(settings));
        if (!settings.ContainsKey(AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH))
            throw new ArgumentNullException(nameof(settings), 
                                            AnalysisLogs.SETTINGS_DOES_NOT_CONTAIN_REQUIRED_KEY + AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH);
        if (!TimeSpan.TryParse(settings[AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH], out var minimalGap)) {
            throw new ArgumentNullException(nameof(settings), 
                                            string.Format(AnalysisLogs.SETTINGS_HAS_WRONG_VALUE, 
                                                          AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH,
                                                          settings[AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH]));
        }
        MinimalGap = minimalGap;
    }
}