using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.GapAnalysis;

[ExcludeFromCodeCoverage]
public class GapSettingsTests
{
    private readonly GapSettings _gapSettings = new();

    [Fact]
    public void Given_Empty_Dictionary_Should_Throw_ArgumentNullException() {
        var dict = new Dictionary<string, string>();
        Assert.Throws<ArgumentNullException>(() => _gapSettings.FromDictionary(dict));
    }
    
    [Fact]
    public void Given_No_MinimalGap_Should_Throw_ArgumentNullException() {
        var dict = new Dictionary<string, string>() {
            { "someKey", "value"}
        };
        Assert.Throws<ArgumentNullException>(() => _gapSettings.FromDictionary(dict));
    }
    
    [Fact]
    public void Given_Wrong_MinimalGap_Dictionary_Should_Throw_ArgumentNullException() {
        var dict = new Dictionary<string, string>() {
            { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "value"}
        };
        Assert.Throws<ArgumentNullException>(() => _gapSettings.FromDictionary(dict));
    }
    
    [Fact]
    public void Given_1d_MinimalGap_Dictionary_Should_Parse_Correctly() {
        var dict = new Dictionary<string, string>() {
            { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1.00:00:00"}
        };
        _gapSettings.FromDictionary(dict);
        _gapSettings.MinimalGap.Days.Should().Be(1);
        _gapSettings.MinimalGap.Hours.Should().Be(0);
    }
    
    [Fact]
    public void Given_1h_MinimalGap_Dictionary_Should_Parse_Correctly() {
        var dict = new Dictionary<string, string>() {
            { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1:00:00"}
        };
        _gapSettings.FromDictionary(dict);
        _gapSettings.MinimalGap.Days.Should().Be(0);
        _gapSettings.MinimalGap.Hours.Should().Be(1);
    }
    
    [Fact]
    public void Given_Dictionary_In_Ctor_Should_Parse_Correctly() {
        var dict = new Dictionary<string, string>() {
            { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1:00:00"}
        };
        var gapSettings = new GapSettings(dict);
        gapSettings.MinimalGap.Days.Should().Be(0);
        gapSettings.MinimalGap.Hours.Should().Be(1);
    }
}