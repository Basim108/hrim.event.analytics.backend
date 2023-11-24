using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis;

[ExcludeFromCodeCoverage]
public class GetAnalysisByEventTypeIdTests: BaseCqrsTests
{
    private readonly HrimFeature _countFeature;
    private readonly HrimFeature _gapFeature;

    public GetAnalysisByEventTypeIdTests() {
        _gapFeature   = TestData.Features.EnsureExistence("FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   true);
        _countFeature = TestData.Features.EnsureExistence("FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true, "explanation");
    }
    
    [Fact]
    public async Task Given_EventType_Created_By_Another_User_Should_Forbid_And_Return_EmptyList() {
        var anotherUserId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var eventType = TestData.Events.CreateEventType(anotherUserId, "Test Type");

        var resultList = await Mediator.Send(new GetAnalysisByEventTypeId(Context: OperatorContext, EventTypeId: eventType.Bl.Id));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Forbidden);
        resultList.Result.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Given_EventType_With_All_On_Analysis_Returns_Them() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.GAP_ANALYSIS,   true, null);

        var resultList = await Mediator.Send(new GetAnalysisByEventTypeId(Context: OperatorContext, EventTypeId: eventType.Bl.Id));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        resultList.Result!.Count.Should().Be(2);
        resultList.Result!.Any(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS).Should().BeTrue();
        resultList.Result!.Any(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS).Should().BeTrue();
    }

    [Fact]
    public async Task Given_EventType_With_All_Off_Analysis_Returns_Them() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.COUNT_ANALYSIS, false, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.GAP_ANALYSIS,   false, null);

        var resultList = await Mediator.Send(new GetAnalysisByEventTypeId(Context: OperatorContext, EventTypeId: eventType.Bl.Id));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        resultList.Result!.Count.Should().Be(2);
        resultList.Result!.Any(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS).Should().BeTrue();
        resultList.Result!.Any(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS).Should().BeTrue();
    }

    [Fact]
    public async Task Given_Gap_Feature_Off_Should_Not_Return_It() {
        _gapFeature.IsOn = false;
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.GAP_ANALYSIS,   true, null);

        var resultList = await Mediator.Send(new GetAnalysisByEventTypeId(Context: OperatorContext, EventTypeId: eventType.Bl.Id));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        resultList.Result!.Count.Should().Be(1);
        resultList.Result!.Any(x => x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS).Should().BeTrue();
        resultList.Result!.Any(x => x.AnalysisCode == FeatureCodes.GAP_ANALYSIS).Should().BeFalse();
    }
    
    [Fact]
    public async Task Given_All_Features_Off_Should_Return_EmptyList() {
        _gapFeature.IsOn   = false;
        _countFeature.IsOn = false;
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Bl.Id, FeatureCodes.GAP_ANALYSIS,   true, null);

        var resultList = await Mediator.Send(new GetAnalysisByEventTypeId(Context: OperatorContext, EventTypeId: eventType.Bl.Id));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().BeNullOrEmpty();
    }
}