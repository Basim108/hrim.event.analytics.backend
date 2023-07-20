using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.Cqrs.Analysis;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis;

[ExcludeFromCodeCoverage]
public class UpdateAnalysisForEventTypeTests: BaseCqrsTests
{
    private readonly HrimFeature _countFeature;
    private readonly HrimFeature _gapFeature;

    public UpdateAnalysisForEventTypeTests() {
        _gapFeature   = TestData.Features.EnsureExistence("FEAT_GAP",   FeatureCodes.GAP_ANALYSIS,   true);
        _countFeature = TestData.Features.EnsureExistence("FEAT_COUNT", FeatureCodes.COUNT_ANALYSIS, true, "explanation");
    }

    [Fact]
    public void CheckDifferences_Should_Check_Only_IsOn_And_Settings() {
        var countDb = new AnalysisByEventType {
            EventTypeId = Guid.NewGuid(),
            CreatedAt   = DateTime.UtcNow,
            Settings = new Dictionary<string, string>() {
                {
                    "key", "value"
                }
            }
        };
        var countIncoming = new AnalysisByEventType {
            Settings = new Dictionary<string, string>() {
                {
                    "key", "value"
                }
            }
        };
        var isDiffer = UpdateAnalysisForEventTypeHandler.CheckDifferences(countDb, countIncoming);
        isDiffer.Should().BeFalse();

        // one key differs
        countDb.Settings = new Dictionary<string, string>() {
            {
                "key", "value"
            }
        };
        countIncoming.Settings = new Dictionary<string, string>() {
            {
                "key1", "value"
            }
        };
        isDiffer = UpdateAnalysisForEventTypeHandler.CheckDifferences(countDb, countIncoming);
        isDiffer.Should().BeTrue();

        // isOn differs
        countDb.IsOn = true;
        countIncoming.Settings = new Dictionary<string, string>() {
            {
                "key", "value"
            }
        };
        isDiffer = UpdateAnalysisForEventTypeHandler.CheckDifferences(countDb, countIncoming);
        isDiffer.Should().BeTrue();
    }

    [Fact]
    public void CheckDifferences_Given_DB_Additional_Settings_Should_Return_True() {
        var countDb = new AnalysisByEventType {
            EventTypeId = Guid.NewGuid(),
            CreatedAt   = DateTime.UtcNow,
            Settings = new Dictionary<string, string>() {
                {
                    "key", "value"
                }, {
                    "additional_key", "additional_value"
                }
            }
        };
        var countIncoming = new AnalysisByEventType {
            Settings = new Dictionary<string, string>() {
                {
                    "key", "value"
                }
            }
        };
        var isDiffer = UpdateAnalysisForEventTypeHandler.CheckDifferences(countDb, countIncoming);
        isDiffer.Should().BeTrue();
    }

    [Fact]
    public async Task Given_EventType_Created_By_Another_User_Should_Forbid_And_DoNot_Update() {
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        var eventType = TestData.Events.CreateEventType(anotherUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        var list = new List<AnalysisByEventType>() {
            new () {
                EventTypeId     = eventType.Id,
                AnalysisCode    = FeatureCodes.COUNT_ANALYSIS,
                ConcurrentToken = 1
            }
        };
        list[0].IsOn = false;
        var resultList = await Mediator.Send(new UpdateAnalysisForEventType(eventType.Id, list, OperatorContext));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Forbidden);
        resultList.Result.Should().BeNullOrEmpty();

        var db = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.COUNT_ANALYSIS);
        db.Should().NotBeNull();
        db!.IsOn.Should().BeTrue();
    }
    
    [Fact]
    public async Task Given_Not_Existed_EventType_Should_Return_NotFound_And_DoNot_Update() {
        var unknownEventTypeId = Guid.NewGuid();
        var list = new List<AnalysisByEventType>() {
            new () {
                EventTypeId     = unknownEventTypeId,
                AnalysisCode    = FeatureCodes.COUNT_ANALYSIS,
                ConcurrentToken = 1
            }
        };
        var resultList = await Mediator.Send(new UpdateAnalysisForEventType(unknownEventTypeId, list, OperatorContext));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.NotFound);
        resultList.Result.Should().BeNullOrEmpty();
    }
    
    [Fact]
    public async Task Given_2_Changed_Analysis_Should_Update_Both() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.GAP_ANALYSIS, true, null);
        var list = new List<AnalysisByEventType>() {
            new () { AnalysisCode = FeatureCodes.COUNT_ANALYSIS, ConcurrentToken = 1 },
            new () { AnalysisCode = FeatureCodes.GAP_ANALYSIS, ConcurrentToken = 1 }
        };
        var beforeUpdate = DateTime.UtcNow;
        var resultList   = await Mediator.Send(new UpdateAnalysisForEventType(eventType.Id, list, OperatorContext));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        var countDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.COUNT_ANALYSIS);
        countDb.Should().NotBeNull();
        countDb!.UpdatedAt.Should().BeAfter(beforeUpdate);
        countDb.IsOn.Should().BeFalse();
        countDb.ConcurrentToken.Should().Be(2);
        var gapDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.GAP_ANALYSIS);
        gapDb.Should().NotBeNull();
        gapDb!.UpdatedAt.Should().BeAfter(beforeUpdate);
        gapDb.IsOn.Should().BeFalse();
        gapDb.ConcurrentToken.Should().Be(2);
    }
    
    [Fact]
    public async Task Given_1_Changed_Analysis_Should_Update_One_And_DoNot_Change_Unchanged() {
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        var count = TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        var list = new List<AnalysisByEventType>() {
            count,
            new () { AnalysisCode = FeatureCodes.GAP_ANALYSIS, ConcurrentToken = 1 }
        };
        var beforeUpdate = DateTime.UtcNow;
        var resultList   = await Mediator.Send(new UpdateAnalysisForEventType(eventType.Id, list, OperatorContext));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        // count should be unchanged
        var countDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.COUNT_ANALYSIS);
        countDb.Should().NotBeNull();
        countDb!.UpdatedAt.Should().Be(countDb.CreatedAt);
        countDb.IsOn.Should().BeTrue();
        countDb.ConcurrentToken.Should().Be(1);
        
        // while gap should be updated
        var gapDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.GAP_ANALYSIS);
        gapDb.Should().NotBeNull();
        gapDb!.UpdatedAt.Should().BeAfter(beforeUpdate);
        gapDb.IsOn.Should().BeFalse();
        gapDb.ConcurrentToken.Should().Be(2);
    }
    
    [Fact]
    public async Task Given_2_Unchanged_Analysis_Should_DoNot_Change_Unchanged() {
        var eventType    = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        var count        = TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        var gap          = TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        var list         = new List<AnalysisByEventType>() { count, gap };
        var resultList   = await Mediator.Send(new UpdateAnalysisForEventType(eventType.Id, list, OperatorContext));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        // count should be unchanged
        var countDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.COUNT_ANALYSIS);
        countDb.Should().NotBeNull();
        countDb!.UpdatedAt.Should().Be(countDb.CreatedAt);
        countDb.IsOn.Should().BeTrue();
        countDb.ConcurrentToken.Should().Be(1);
        
        // gap should be unchanged
        var gapDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.GAP_ANALYSIS);
        gapDb.Should().NotBeNull();
        gapDb!.UpdatedAt.Should().Be(gapDb.CreatedAt);
        gapDb.IsOn.Should().BeTrue();
        gapDb.ConcurrentToken.Should().Be(1);
    }
    
    [Fact]
    public async Task Given_2_Changed_Analysis_When_Count_Feature_IsOff_Should_Update_Both() {
        _countFeature.IsOn = false;
        var eventType = TestData.Events.CreateEventType(OperatorUserId, "Test Type");
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.COUNT_ANALYSIS, true, null);
        TestData.AnalysisByEventType.EnsureExistence(eventType.Id, FeatureCodes.GAP_ANALYSIS,   true, null);
        var list = new List<AnalysisByEventType>() {
            new () { AnalysisCode = FeatureCodes.COUNT_ANALYSIS, ConcurrentToken = 1 },
            new () { AnalysisCode = FeatureCodes.GAP_ANALYSIS, ConcurrentToken   = 1 }
        };
        var beforeUpdate = DateTime.UtcNow;
        var resultList   = await Mediator.Send(new UpdateAnalysisForEventType(eventType.Id, list, OperatorContext));
        resultList.Should().NotBeNull();
        resultList.StatusCode.Should().Be(CqrsResultCode.Ok);
        resultList.Result.Should().NotBeNullOrEmpty();

        var countDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.COUNT_ANALYSIS);
        countDb.Should().NotBeNull();
        countDb!.UpdatedAt.Should().BeAfter(beforeUpdate);
        countDb.IsOn.Should().BeFalse();
        countDb.ConcurrentToken.Should().Be(2);
        var gapDb = await TestData.AnalysisByEventType.GetAsync(eventType.Id, FeatureCodes.GAP_ANALYSIS);
        gapDb.Should().NotBeNull();
        gapDb!.UpdatedAt.Should().BeAfter(beforeUpdate);
        gapDb.IsOn.Should().BeFalse();
        gapDb.ConcurrentToken.Should().Be(2);
    }
}