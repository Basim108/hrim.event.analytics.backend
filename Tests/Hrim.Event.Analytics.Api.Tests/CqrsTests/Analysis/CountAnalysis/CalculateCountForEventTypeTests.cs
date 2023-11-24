using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;
using Hrim.Event.Analytics.Analysis.Models;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.CountAnalysis;

[ExcludeFromCodeCoverage]
public class CalculateCountForEventTypeTests: BaseCqrsTests
{
    private readonly ICountEventHierarchyAccessor      _hierarchyAccessor  = Substitute.For<ICountEventHierarchyAccessor>();
    private readonly ICountCalculationService          _calculationService = Substitute.For<ICountCalculationService>();
    private readonly CalculateCountForEventTypeHandler _handler;

    public CalculateCountForEventTypeTests() {
        _handler = new CalculateCountForEventTypeHandler(NullLogger<CalculateCountForEventTypeHandler>.Instance, 
                                                         _calculationService,
                                                         _hierarchyAccessor);
    }

    [Fact]
    public async Task Given_0_As_EventTypeId_Should_Throe_ArgumentNullException() {
        var calcInfo = new EventTypeAnalysisSettings(0, null, DateTime.UtcNow, default);
        var command  = new CalculateCountForEventType(calcInfo, null);
        var ex       = await Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.Send(command));
        ex.ParamName.Should().Be("request");
        ex.Message.Contains("EventTypeId").Should().BeTrue();
    }
    
    /// <summary>
    /// CASE 1: there are no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
    /// </summary>
    [Fact]
    public async Task Case1_Given_LastRun_When_All_Events_Deleted_Should_Return_Both_Counts_0() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent>());
        _hierarchyAccessor.CountDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(0);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(-2),
            FinishedAt   = DateTime.UtcNow.AddMinutes(-1)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Any<List<AnalysisEvent>>(), Arg.Any<int>());
    }
    
    /// <summary>
    /// CASE 2: there are no events and no last run  => do nothing even analysis_result should be null
    /// </summary>
    [Fact]
    public async Task Case2_Given_No_LastRun_When_No_Events_Should_Return_Null() {
        var eventType    = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns((DateTime?)null);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns((DateTime?)null);
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, null), CancellationToken.None);
        _calculationService.Received(0).Calculate(Arg.Any<List<AnalysisEvent>>(), Arg.Any<int>());
    }
    
    /// <summary>
    /// CASE 3: there are no changes before last run and there are changes after last run => recalculate everything
    /// </summary>
    [Fact]
    public async Task Case3_Given_Changes_After_LastRun_Should_Return_Calculate_It() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration = TestData.Events.CreateDurationEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.CountDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(1);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddSeconds(-1),
            FinishedAt   = DateTime.UtcNow.AddMicroseconds(1)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 1), Arg.Is(1));
    }
    
    /// <summary>
    /// CASE 4: there are changes of event created before last run => recalculate everything
    /// </summary>
    [Fact]
    public async Task Case4_Given_Changes_For_Events_Before_LastRun_Should_Calculate_It() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration  = TestData.Events.CreateDurationEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.CountDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(1);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(-2),
            FinishedAt   = DateTime.UtcNow.AddMinutes(-1)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 1), Arg.Is(1));
    }
    
    /// <summary>
    /// CASE 5: there are no changes neither before nor after last run => no calculation required
    /// </summary>
    [Fact]
    public async Task Case5_Given_No_Changes_Should_Return_Null() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration  = TestData.Events.CreateDurationEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow.AddMinutes(-1));
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow.AddMinutes(-1));
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.CountDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(1);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType.Bl.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(1),
            FinishedAt   = DateTime.UtcNow.AddMinutes(2)
        };
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, lastRun), CancellationToken.None);
        _calculationService.Received(0).Calculate(Arg.Any<List<AnalysisEvent>>(), Arg.Any<int>());
    }
    
    /// <summary>
    /// CASE 7: there is no durations, but some occurrences
    /// </summary>
    [Fact]
    public async Task Case7_Given_Occurrence_But_No_Duration_Should_Return_Occurrences() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns((DateTime?)null);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent>());
        _hierarchyAccessor.CountDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(1);
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, null), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 0), Arg.Is(1));
    }
    
    /// <summary>
    /// CASE 8: there is no occurrences but some durations
    /// </summary>
    [Fact]
    public async Task Case8_Given_Durations_But_No_Occurrences_Should_Return_Occurrences() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type #1");
        var duration  = TestData.Events.CreateDurationEvent(eventType.Bl.CreatedById, eventType.Bl.Id);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbDurationEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(DateTime.UtcNow);
        _hierarchyAccessor.GetLastUpdatedEventTimeAsync<DbOccurrenceEvent>(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns((DateTime?)null);
        _hierarchyAccessor.GetDescendantDurationsAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(new List<AnalysisEvent> {
                               new (duration.StartedOn, duration.StartedAt, duration.FinishedOn, duration.FinishedAt)
                           });
        _hierarchyAccessor.CountDescendantOccurrencesAsync(Arg.Is(eventType.Db.TreeNodePath!.Value), Arg.Any<CancellationToken>())
                          .Returns(0);
        var eventTypeInfo = new EventTypeAnalysisSettings(eventType.Bl.Id, null, eventType.Bl.UpdatedAt!.Value, eventType.Db.TreeNodePath!.Value);
        await _handler.Handle(new CalculateCountForEventType(eventTypeInfo, null), CancellationToken.None);
        _calculationService.Received(1).Calculate(Arg.Is<List<AnalysisEvent>>(x => x.Count == 1), Arg.Is(0));
    }
}