namespace Hrim.Event.Analytics.Analysis;

public static class AnalysisSettingNames
{
    public static class Gap
    {
        public const string MINIMAL_GAP_LENGTH = "minimal_gap_length";
        
        public static readonly List<string> AllProps = new() {
            MINIMAL_GAP_LENGTH
        };
    }
}