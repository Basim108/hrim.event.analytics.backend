using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;
using Hrim.Event.Analytics.Analysis.Models;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.GapAnalysis;

[ExcludeFromCodeCoverage]
public class CalculateGapForEventTypeTests: BaseCqrsTests
{
    private readonly Dictionary<string, string> _settings = new () {
        { AnalysisSettingNames.Gap.MINIMAL_GAP_LENGTH, "1.00:00:00" }
    };
    
    private readonly IGapEventHierarchyAccessor      _hierarchyAccessor  = Substitute.For<IGapEventHierarchyAccessor>();
    private readonly IGapCalculationService          _calculationService = Substitute.For<IGapCalculationService>();
    private readonly CalculateGapForEventTypeHandler _handler;

    public CalculateGapForEventTypeTests() {
        _handler = new CalculateGapForEventTypeHandler(NullLogger<CalculateGapForEventTypeHandler>.Instance, 
                                                       _calculationService,
                                                       _hierarchyAccessor);
    }

    [Fact]
    public async Task Given_0_As_EventTypeId_Should_Throe_ArgumentNullException() {
        var calcInfo = new EventTypeAnalysisSettings(0, _settings, DateTime.UtcNow, default);
        var command  = new CalculateGapForEventType(calcInfo, null);
        var ex       = await Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.Send(command));
        ex.ParamName.Should().Be("request");
        ex.Message.Contains("EventTypeId").Should().BeTrue();
    }

    /// <summary>
    /// CASE 1: there are no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
    /// </summary>
    [Fact]
    public async Task Case1_Given_LastRun_When_All_Events_Deleted_Should_Return_EventCount_0() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1").Db;
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent>());
        _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent>());
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(-2),
            FinishedAt   = DateTime.UtcNow.AddMinutes(-1)
        };
        var calcInfo = new EventTypeAnalysisSettings(eventType.Id, _settings, DateTime.MinValue, eventType.TreeNodePath);
        await _handler.Handle(new CalculateGapForEventType(calcInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 0), Arg.Any<GapSettings>());
    }
    
    /// <summary>
    /// CASE 2: there are no events and no last run  => do nothing even analysis_result should be null
    /// </summary>
    [Fact]
    public async Task Case2_Given_No_LastRun_When_No_Events_Should_Return_Null() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1").Db;
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns((DateTime?)null);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns((DateTime?)null);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent>());
        _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent>());
        var calcInfo = new EventTypeAnalysisSettings(eventType.Id, _settings, DateTime.MinValue, eventType.TreeNodePath);
        await _handler.Handle(new CalculateGapForEventType(calcInfo, null), CancellationToken.None);
        _calculationService.Received(0).Calculate(Arg.Any<List<AnalysisEvent>>(), Arg.Any<GapSettings>());
    }
    
    /// <summary>
    /// CASE 3: there are no changes in events created before last run and there are changes in event created after last run => recalculate everything
    /// </summary>
    [Fact]
    public async Task Case3_Given_Changes_After_LastRun_Should_Return_Calculate_It() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration  = TestData.Events.CreateDurationEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        var occurrence  = TestData.Events.CreateOccurrenceEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (occurrence.OccurredOn, occurrence.OccurredAt, null, null)
                           });
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddSeconds(-1),
            FinishedAt   = DateTime.UtcNow.AddMicroseconds(1)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, _settings, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath);
        await _handler.Handle(new CalculateGapForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 3), Arg.Any<GapSettings>());
    }
    
    /// <summary>
    /// CASE 4: there are changes in events created before last run  => recalculate everything
    /// </summary>
    [Fact]
    public async Task Case4_Given_Changes_For_Events_Before_LastRun_Should_Calculate_It() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration  = TestData.Events.CreateDurationEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        var occurrence  = TestData.Events.CreateOccurrenceEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (occurrence.OccurredOn, occurrence.OccurredAt, null, null)
                           });
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(-2),
            FinishedAt   = DateTime.UtcNow.AddMinutes(-1)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, _settings, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath);
        await _handler.Handle(new CalculateGapForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 3), Arg.Any<GapSettings>());
    }
    
    /// <summary>
    /// CASE 5: there are no changes neither before nor after last run => no calculation required
    /// </summary>
    [Fact]
    public async Task Case5_Given_No_Changes_Should_Return_Null() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow.AddMinutes(-1));
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow.AddMinutes(-1));
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> ());
        _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> ());
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(1),
            FinishedAt   = DateTime.UtcNow.AddMinutes(2)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, _settings, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath);
        await _handler.Handle(new CalculateGapForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(0).Calculate(Arg.Any<List<AnalysisEvent>>(), Arg.Any<GapSettings>());
    }
    
    /// <summary>
    /// CASE 8: there are no changes in events, but analysis settings changed after last run => recalculate
    /// </summary>
    [Fact]
    public async Task Case8_Given_No_Changed_Events_When_Settings_Changed_After_LastRun_Should_Calculate() {
        var eventType  = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1").Db;
        var duration   = TestData.Events.CreateDurationEvent(eventType.CreatedById, eventType.Id);
        var occurrence = TestData.Events.CreateOccurrenceEvent(eventType.CreatedById, eventType.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow.AddMinutes(-1));
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow.AddMinutes(-1));
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (occurrence.OccurredOn, occurrence.OccurredAt, null, null)
                           });
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(1),
            FinishedAt   = DateTime.UtcNow.AddMinutes(2)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Id, _settings, DateTime.UtcNow.AddSeconds(61), eventType.TreeNodePath);
        await _handler.Handle(new CalculateGapForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 3), Arg.Any<GapSettings>());
    }
    
    // TODO: move to test of CalculationGapService
    // [Fact]
    // public async Task Given_Occurrence_Events_With_2_Gaps_Then_Calculate_Them() {
    //     var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1").Db;
    //     var firstDt        = DateTimeOffset.Now.AddDays(-11);
    //     var event1         = TestData.Events.CreateOccurrenceEvent(eventType.CreatedById, eventType.Id, occurredAt: firstDt);
    //     var event2         = TestData.Events.CreateOccurrenceEvent(eventType.CreatedById, eventType.Id, occurredAt: event1.OccurredAt.AddHours(2));
    //     var event3         = TestData.Events.CreateOccurrenceEvent(eventType.CreatedById, eventType.Id, occurredAt: event2.OccurredAt.AddDays(3));
    //     var event4         = TestData.Events.CreateOccurrenceEvent(eventType.CreatedById, eventType.Id, occurredAt: event3.OccurredAt.AddDays(7));
    //     _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
    //                       .Returns(DateTime.UtcNow.AddMinutes(-1));
    //     _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
    //                       .Returns(DateTime.UtcNow.AddMinutes(-1));
    //     _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
    //                       .Returns(new List<AnalysisEvent>());
    //     _hierarchyAccessor.GetDescendantOccurrencesAsync(Arg.Is(eventType.TreeNodePath), Arg.Any<CancellationToken>())
    //                       .Returns(new List<AnalysisEvent> {
    //                            new (event1.OccurredOn, event1.OccurredAt, null, null),
    //                            new (event2.OccurredOn, event2.OccurredAt, null, null),
    //                            new (event3.OccurredOn, event3.OccurredAt, null, null),
    //                            new (event4.OccurredOn, event4.OccurredAt, null, null)
    //                        });
    //     var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Id, _settings, DateTime.MinValue, eventType.TreeNodePath);
    //     await _handler.Handle(new CalculateGapForEventType(eventTypeInfo, null), CancellationToken.None);
    //     _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 3), Arg.Any<GapSettings>());
    //     result.Should().NotBeNull();
    //     result!.EventCount.Should().Be(4);
    //     result.GapCount.Should().Be(2);
    //     result.Min.Should().NotBeNull();
    //     result.Min.Should().Be(TimeSpan.FromDays(3));
    //     result.MinGapDate.Should().NotBeNull();
    //     result.MinGapDate.Should().Be(event2.OccurredOn);
    //     result.Max.Should().NotBeNull();
    //     result.Max.Should().Be(TimeSpan.FromDays(7));
    //     result.MaxGapDate.Should().NotBeNull();
    //     result.MaxGapDate.Should().Be(event3.OccurredOn);
    //     result.Avg.Should().NotBeNull();
    // }
}