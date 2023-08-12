using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis;

[ExcludeFromCodeCoverage]
public class SyncAnalysisSettingsTests: BaseCqrsTests
{
    private readonly HrimFeature       _gapFeature;
    private readonly List<HrimFeature> _features;

    public SyncAnalysisSettingsTests() {
        _gapFeature   = TestData.Features.EnsureExistence("FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   true);
        var countFeature = TestData.Features.EnsureExistence("FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true, "explanation");
        _features = new List<HrimFeature>() { countFeature, _gapFeature };
    }

    [Fact]
    public async Task Given_EventType_With_Count_Settings_Should_Create_Missing_Settings() {
        var eventType     = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        var countSettings = TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.COUNT_ANALYSIS, true, null);

        var command = new SyncAnalysisSettings(eventType.Id,
                                               new List<AnalysisByEventType>() { countSettings },
                                               _features,
                                               IsSaveChanges: true);
        var resultList = await Mediator.Send(command);
        resultList.Should().NotBeNull();
        resultList!.Count.Should().Be(1);
        resultList[0].AnalysisCode.Should().Be(FeatureCodes.GAP_ANALYSIS);
        resultList[0].ConcurrentToken.Should().Be(1);
    }

    [Fact]
    public async Task Given_EventType_With_Gap_Settings_Should_Create_Missing_Settings() {
        var eventType     = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        var countSettings = TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.GAP_ANALYSIS, true, null);

        var command = new SyncAnalysisSettings(eventType.Id,
                                               new List<AnalysisByEventType>() { countSettings },
                                               _features,
                                               IsSaveChanges: true);
        var resultList = await Mediator.Send(command);
        resultList.Should().NotBeNull();
        resultList!.Count.Should().Be(1);
        resultList[0].AnalysisCode.Should().Be(FeatureCodes.COUNT_ANALYSIS);
    }

    [Fact]
    public async Task Given_EventType_With_No_Settings_Should_Create_Settings_Of_Only_Available_Features() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        _gapFeature.IsOn = false;
        TestData.DbContext.SaveChanges();

        var command    = new SyncAnalysisSettings(eventType.Id, null, _features, IsSaveChanges: true);
        var resultList = await Mediator.Send(command);
        resultList.Should().NotBeNull();
        resultList!.Count.Should().Be(1);
        resultList[0].AnalysisCode.Should().Be(FeatureCodes.COUNT_ANALYSIS);
    }

    [Fact]
    public async Task Given_EventType_With_No_Settings_Should_Create_All_Settings() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");

        var command    = new SyncAnalysisSettings(eventType.Id, null, _features, IsSaveChanges: true);
        var resultList = await Mediator.Send(command);
        resultList.Should().NotBeNull();
        resultList!.Count.Should().Be(2);
        resultList.Any(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS).Should().BeTrue();
        resultList.Any(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS).Should().BeTrue();
    }
}