﻿using System.Collections.Immutable;

namespace Hrim.Event.Analytics.Analysis;

public static class AnalysisSettingNames
{
    public static class Gap
    {
        public const string MINIMAL_GAP_LENGTH = "minimal_gap_length";

        public static readonly ImmutableList<string> AllProps = ImmutableList.Create(MINIMAL_GAP_LENGTH);
    }
}