using System.Collections.Immutable;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions;

public static class FeatureCodes
{
    public const string COUNT_ANALYSIS = "count";
    public const string GAP_ANALYSIS   = "gap";

    public static readonly ImmutableList<string> AllCodes = ImmutableList.Create(COUNT_ANALYSIS,
                                                                                 GAP_ANALYSIS);
}