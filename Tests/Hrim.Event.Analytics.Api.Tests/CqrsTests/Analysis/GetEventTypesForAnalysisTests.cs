using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis;

[ExcludeFromCodeCoverage]
public class GetEventTypesForAnalysisTests: BaseCqrsTests
{
    private readonly HrimFeature _countFeature;
    private readonly HrimFeature _gapFeature;

    public GetEventTypesForAnalysisTests() {
        _gapFeature   = TestData.Features.EnsureExistence("FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   true);
        _countFeature = TestData.Features.EnsureExistence("FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true, "explanation");
    }
    
    [Fact]
    public async Task Given_Empty_AnalysisCode_Should_Throw_ArgumentNullException() {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.Send(new GetEventTypesForAnalysis("")));
        ex.Should().NotBeNull();
        ex.ParamName.Should().Be("request");
        ex.Message.Should().Contain("AnalysisCode");
    }
    
    [Fact]
    public async Task Given_Gap_Analysis_When_Feature_On_Should_Return_Only_For_Gaps() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        var eventType2 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        TestData.AnalysisByEventType.EnsureExistence(eventType1.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType1.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType2.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType2.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        
        var resultList = await Mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.GAP_ANALYSIS));

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(2);
        resultList.Any(x => x.EventTypeId == eventType1.Id).Should().BeTrue();
        resultList.Any(x => x.EventTypeId == eventType2.Id).Should().BeTrue();
    }
    
    [Fact]
    public async Task Given_Gap_Analysis_When_Feature_Off_Should_Return_Empty_List() {
        _gapFeature.IsOn   = false;
        _countFeature.IsOn = false;
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        var eventType2 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        TestData.AnalysisByEventType.EnsureExistence(eventType1.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType1.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType2.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType2.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        
        var resultList = await Mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.GAP_ANALYSIS));

        resultList.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Given_Gap_Analysis_When_Analysis_Off_Should_Skip_This_EventTypes() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        var eventType2 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        TestData.AnalysisByEventType.EnsureExistence(eventType1.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType1.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType2.Id, FeatureCodes.GAP_ANALYSIS,   false, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType2.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        
        var resultList = await Mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.GAP_ANALYSIS));

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(1);
        resultList[0].EventTypeId.Should().Be(eventType1.Id);
    }
    
    [Fact]
    public async Task Given_Gap_Analysis_Should_Return_Its_Settings() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Type").Bl;
        var settings  = new Dictionary<string, string>() {
            { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "01:00:00"}
        };
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.GAP_ANALYSIS, true, settings);
        
        var resultList = await Mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.GAP_ANALYSIS));

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(1);
        resultList[0].EventTypeId.Should().Be(eventType.Id);
        resultList[0].Settings.Should().NotBeEmpty();
        resultList[0].Settings!.ContainsKey(AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH).Should().BeTrue();
    }
    
    [Fact]
    public async Task Given_EventType_When_They_Have_Children_That_Does_Not_Have_Settings_Should_Return_With_Children() {
        var anotherEventType1 = TestData.Events.CreateEventType(OperatorUserId, "Test Type").Bl;
        var anotherEventType2 = TestData.Events.CreateEventType(OperatorUserId, "Test Type").Bl;
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type").Bl;
        var childType = TestData.Events.CreateEventType(OperatorUserId, "Test Child Type", parentId: eventType.Id).Bl;
        TestData.AnalysisByEventType.EnsureExistence(anotherEventType1.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(anotherEventType1.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(anotherEventType2.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(anotherEventType2.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id,         FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id,         FeatureCodes.GAP_ANALYSIS,   true, null);
        
        var resultList = await Mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.GAP_ANALYSIS));

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(3);
        resultList.Any(x => x.EventTypeId == eventType.Id).Should().BeTrue();
        resultList.First(x => x.EventTypeId == eventType.Id).TreeNodePath.Should().NotBeNull();
    }
}