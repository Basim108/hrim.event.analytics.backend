using System.Collections.Immutable;

namespace Hrim.Event.Analytics.Abstractions;

#pragma warning disable CS1591
public static class FeatureVars
{
    public const string COUNT_ANALYSIS = "FEAT_COUNT_ANALYSIS";
    public const string GAP_ANALYSIS   = "FEAT_GAP_ANALYSIS";

    public static readonly ImmutableList<string> AllCodes = ImmutableList.Create(COUNT_ANALYSIS,
                                                                                 GAP_ANALYSIS);
}