using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.GapAnalysis;

[ExcludeFromCodeCoverage]
public class CalculateGapForEventTypeTests: BaseCqrsTests
{
    private readonly Dictionary<string, string> _settings = new () {
        { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1.00:00:00" }
    };

    [Fact]
    public async Task Should() {
        var calcInfo = new EventTypeAnalysisSettings(0, _settings, DateTime.UtcNow, Enumerable.Empty<long>());
        var command  = new CalculateGapForEventType(calcInfo, null);
        var ex       = await Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.Send(command));
        ex.ParamName.Should().Be("request");
        ex.Message.Contains("EventTypeId").Should().BeTrue();
    }

    /// <summary>
    /// CASE 1: there is no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
    /// </summary>
    [Fact]
    public async Task Given_LastRun_When_All_Events_Deleted_Should_Return_EventCount_0() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, isDeleted: true);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, isDeleted: true);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(-2),
            FinishedAt   = DateTime.UtcNow.AddMinutes(-1)
        };
        var calcInfo = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result   = await Mediator.Send(new CalculateGapForEventType(calcInfo, lastRun));
        result.Should().NotBeNull();
        result!.EventCount.Should().Be(0);
        result.Min.Should().BeNull();
        result.MinGapDate.Should().BeNull();
        result.Max.Should().BeNull();
        result.MaxGapDate.Should().BeNull();
        result.Avg.Should().BeNull();
        result.GapCount.Should().Be(0);
    }
    
    /// <summary>
    /// CASE 2: there is no events and no last run  => do nothing even analysis_result should be null
    /// </summary>
    [Fact]
    public async Task Given_No_LastRun_When_No_Events_Should_Return_Null() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var calcInfo   = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result     = await Mediator.Send(new CalculateGapForEventType(calcInfo, null));
        result.Should().BeNull();
    }
    
    /// <summary>
    /// CASE 3: there is no changes before last run and there are changes after last run => recalculate everything
    /// </summary>
    [Fact]
    public async Task Given_Changes_After_LastRun_Should_Return_Calculate_It() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow,
            FinishedAt   = DateTime.UtcNow.AddMicroseconds(1)
        };
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var calcInfo = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result   = await Mediator.Send(new CalculateGapForEventType(calcInfo, lastRun));
        result.Should().NotBeNull();
        result!.EventCount.Should().Be(4);
    }
    
    /// <summary>
    /// CASE 4: there is changes before last run  => recalculate everything
    /// </summary>
    [Fact]
    public async Task Given_Changes_For_Events_Before_LastRun_Should_Calculate_It() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration   = TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow,
            FinishedAt   = DateTime.UtcNow.AddMicroseconds(1)
        };
        duration.UpdatedAt = DateTime.UtcNow.AddMinutes(1);
        TestData.DbContext.SaveChanges();
        var calcInfo = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result   = await Mediator.Send(new CalculateGapForEventType(calcInfo, lastRun));
        result.Should().NotBeNull();
        result!.EventCount.Should().Be(2);
    }
    
    /// <summary>
    /// CASE 5: there is no changes neither before nor after last run => no calculation required
    /// </summary>
    [Fact]
    public async Task Given_No_Changes_Should_Return_Null() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(1),
            FinishedAt   = DateTime.UtcNow.AddMinutes(2)
        };
        var calcInfo = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result   = await Mediator.Send(new CalculateGapForEventType(calcInfo, lastRun));
        result.Should().BeNull();
    }

    /// <summary>
    /// CASE 6: there are shuffled event kinds: e.g. sequence of occurrence, duration, occurrence
    /// </summary>
    [Fact]
    public async Task Given_Shuffled_Events_Should_Compare_Occurrence_To_Duration() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var now        = DateTimeOffset.UtcNow;
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: now);
        var firstOccurrence = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: now.AddDays(1));
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: firstOccurrence.OccurredAt.AddDays(1));
        var calcInfo = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result   = await Mediator.Send(new CalculateGapForEventType(calcInfo, null));
        result.Should().NotBeNull();
        result!.EventCount.Should().Be(3);
        result.GapCount.Should().Be(0);
    }
    
    /// <summary>
    /// CASE 7: Ignores deleted events
    /// </summary>
    [Fact]
    public async Task Given_Deleted_Event_Should_Ignore_It() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var now        = DateTimeOffset.UtcNow;
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: now);
        var firstOccurrence = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: now.AddDays(1), isDeleted:true);
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: firstOccurrence.OccurredAt.AddDays(1));
        var calcInfo = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result   = await Mediator.Send(new CalculateGapForEventType(calcInfo, null));
        result.Should().NotBeNull();
        result!.EventCount.Should().Be(2);
        result.GapCount.Should().Be(1);
    }
    
    /// <summary>
    /// CASE 8: there is no changes in events, but analysis settings changed after last run => recalculate
    /// </summary>
    [Fact]
    public async Task Given_No_Changed_Events_When_Settings_Changed_After_LastRun_Should_Calculate() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddSeconds(60),
            FinishedAt   = DateTime.UtcNow.AddSeconds(120)
        };
        var calcInfo      = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.UtcNow.AddSeconds(61), Enumerable.Empty<long>());
        var result        = await Mediator.Send(new CalculateGapForEventType(calcInfo, lastRun));
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Given_Occurrence_Events_With_2_Gaps_Then_Calculate_Them() {
        var eventType1 = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var firstDt    = DateTimeOffset.Now.AddDays(-11);
        var event1     = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: firstDt);
        var event2     = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: event1.OccurredAt.AddHours(2));
        var event3     = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: event2.OccurredAt.AddDays(3));
        var event4     = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: event3.OccurredAt.AddDays(7));
        var calcInfo   = new EventTypeAnalysisSettings(eventType1.Id, _settings, DateTime.MinValue, Enumerable.Empty<long>());
        var result     = await Mediator.Send(new CalculateGapForEventType(calcInfo, null));
        result.Should().NotBeNull();
        result!.EventCount.Should().Be(4);
        result.GapCount.Should().Be(2);
        result.Min.Should().NotBeNull();
        result.Min.Should().Be(TimeSpan.FromDays(3));
        result.MinGapDate.Should().NotBeNull();
        result.MinGapDate.Should().Be(event2.OccurredOn);
        result.Max.Should().NotBeNull();
        result.Max.Should().Be(TimeSpan.FromDays(7));
        result.MaxGapDate.Should().NotBeNull();
        result.MaxGapDate.Should().Be(event3.OccurredOn);
        result.Avg.Should().NotBeNull();
    }
}