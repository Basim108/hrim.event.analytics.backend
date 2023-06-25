#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions;

public static class FeatureCodes
{
    public const string COUNT_ANALYSIS = "count";
    public const string GAP_ANALYSIS = "gap";

    public static readonly List<string> AllCodes = new() {
        COUNT_ANALYSIS,
        GAP_ANALYSIS
    };
}

public static class GapSettings
{
    
}