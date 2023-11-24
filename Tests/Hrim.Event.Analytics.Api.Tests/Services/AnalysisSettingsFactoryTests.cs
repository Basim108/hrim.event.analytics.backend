using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Analysis.Services;

namespace Hrim.Event.Analytics.Api.Tests.Services;

public class AnalysisSettingsFactoryTests
{
    private readonly AnalysisSettingsFactory _factory = new();

    [Fact]
    public void GetDefaultSettings_Should_Return_Settings_For_Gap_Analysis() {
        var result = _factory.GetDefaultSettings();
        result.Should().NotBeEmpty();
        var settings = result.First(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS);
        settings.IsOn.Should().BeTrue();
        settings.Settings.Should().NotBeEmpty();
        settings.Settings!.Count.Should().Be(1);
        settings.Settings.ContainsKey(AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH).Should().BeTrue();
        settings.Settings[AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH].Should().Be("1.00:00:00");
    }
    
    [Fact]
    public void GetDefaultSettings_Should_Return_Settings_For_Count_Analysis() {
        var result = _factory.GetDefaultSettings();
        result.Should().NotBeEmpty();
        var settings = result.First(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS);
        settings.IsOn.Should().BeTrue();
        settings.Settings.Should().BeNull();
    }

    [Fact]
    public void GetMissedSettings_Given_Null_Returns_Default_Settings() {
        var result = _factory.GetMissedSettings(null);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_factory.GetDefaultSettings());
    }
    
    [Fact]
    public void GetMissedSettings_Given_Empty_Returns_Default_Settings() {
        var result = _factory.GetMissedSettings(new List<AnalysisConfigByEventType>());
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_factory.GetDefaultSettings());
    }
    
    [Fact]
    public void GetMissedSettings_Given_All_Settings_Returns_Null() {
        var result = _factory.GetMissedSettings(_factory.GetDefaultSettings());
        result.Should().BeNull();
    }
    
    [Fact]
    public void GetMissedSettings_Given_Gap_Settings_Returns_Count_Settings() {
        var settings = new AnalysisConfigByEventType() { AnalysisCode = FeatureCodes.GAP_ANALYSIS };
        var result   = _factory.GetMissedSettings(new List<AnalysisConfigByEventType>{ settings });
        result.Should().NotBeNull();
        result!.Count.Should().Be(1);
        result[0].AnalysisCode.Should().Be(FeatureCodes.COUNT_ANALYSIS);
    }
    
    [Fact]
    public void GetMissedSettings_Given_Count_Settings_Returns_Gap_Settings() {
        var settings = new AnalysisConfigByEventType() { AnalysisCode = FeatureCodes.COUNT_ANALYSIS };
        var result   = _factory.GetMissedSettings(new List<AnalysisConfigByEventType>{ settings });
        result.Should().NotBeNull();
        result!.Count.Should().Be(1);
        result[0].AnalysisCode.Should().Be(FeatureCodes.GAP_ANALYSIS);
    }
}