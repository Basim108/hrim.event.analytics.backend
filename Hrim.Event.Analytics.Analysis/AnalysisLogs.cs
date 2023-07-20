namespace Hrim.Event.Analytics.Analysis;

public static class AnalysisLogs
{
    public const string SETTINGS_DOES_NOT_CONTAIN_REQUIRED_KEY  = "Settings dictionary does not contain required key ";
    public const string SETTINGS_HAS_WRONG_VALUE                = "Settings dictionary has wrong value for Key={0} Value={1}";
    public const string STARTING_AT_IS_GREATER_THAN_FINISHED_AT = "StaringAt is greater than FinishedAt";

    public const string GAP_CALCULATION_PARAMS =
        "Gap Analysis calculation params: IsFirstRun={IsFirstRun}, IsDurationChanged={IsDurationChanged}, IsOccurrenceChanged={IsOccurrenceChanged}, IsCalcSettingsChanged={IsCalcSettingsChanged}";
}