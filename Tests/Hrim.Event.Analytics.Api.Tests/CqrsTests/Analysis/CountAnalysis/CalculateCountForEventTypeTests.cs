using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis.CountAnalysis;

[ExcludeFromCodeCoverage]
public class CalculateCountForEventTypeTests: BaseCqrsTests
{
    [Fact]
    public async Task Should() {
        var command = new CalculateCountForEventType(Guid.Empty, null);
        var ex      = await Assert.ThrowsAsync<ArgumentNullException>(() => Mediator.Send(command));
        ex.ParamName.Should().Be("request");
        ex.Message.Contains("EventTypeId").Should().BeTrue();
    }

    /// <summary>
    /// CASE 1: there is no events (all deleted after last run) => remove pre analysis result from db  => then result.EventCount = 0
    /// </summary>
    [Fact]
    public async Task Given_LastRun_When_All_Events_Deleted_Should_Return_Both_Counts_0() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, isDeleted: true);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, isDeleted: true);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(-2),
            FinishedAt   = DateTime.UtcNow.AddMinutes(-1)
        };
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, lastRun));
        result.Should().NotBeNull();
        result!.OccurrencesCount.Should().Be(0);
        result.MinDuration.Should().BeNull();
        result.MinDurationDate.Should().BeNull();
        result.MaxDuration.Should().BeNull();
        result.MaxDurationDate.Should().BeNull();
        result.AvgDuration.Should().BeNull();
        result.OccurrencesCount.Should().Be(0);
        result.DurationsCount.Should().Be(0);
    }
    
    /// <summary>
    /// CASE 2: there is no events and no last run  => do nothing even analysis_result should be null
    /// </summary>
    [Fact]
    public async Task Given_No_LastRun_When_No_Events_Should_Return_Null() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, null));
        result.Should().BeNull();
    }
    
    /// <summary>
    /// CASE 3: there is no changes before last run and there are changes after last run => recalculate everything
    /// </summary>
    [Fact]
    public async Task Given_Changes_After_LastRun_Should_Return_Calculate_It() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow,
            FinishedAt   = DateTime.UtcNow.AddMicroseconds(1)
        };
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, lastRun));
        result.Should().NotBeNull();
        result!.DurationsCount.Should().Be(2);
        result.OccurrencesCount.Should().Be(2);
    }
    
    /// <summary>
    /// CASE 4: there is changes before last run  => recalculate everything
    /// </summary>
    [Fact]
    public async Task Given_Changes_For_Events_Before_LastRun_Should_Calculate_It() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var duration = TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow,
            FinishedAt   = DateTime.UtcNow.AddMicroseconds(1)
        };
        duration.UpdatedAt = DateTime.UtcNow.AddMinutes(1);
        TestData.DbContext.SaveChanges();
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, lastRun));
        result.Should().NotBeNull();
        result!.DurationsCount.Should().Be(1);
        result.OccurrencesCount.Should().Be(1);
    }
    
    /// <summary>
    /// CASE 5: there is no changes neither before nor after last run => no calculation required
    /// </summary>
    [Fact]
    public async Task Given_No_Changes_Should_Return_Null() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id);
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id);
        var lastRun = new StatisticsForEventType {
            EntityId     = eventType1.Id,
            AnalysisCode = FeatureCodes.COUNT_ANALYSIS,
            StartedAt    = DateTime.UtcNow.AddMinutes(1),
            FinishedAt   = DateTime.UtcNow.AddMinutes(2)
        };
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, lastRun));
        result.Should().BeNull();
    }
    
    /// <summary>
    /// CASE 6: Ignores deleted events
    /// </summary>
    [Fact]
    public async Task Given_Deleted_Event_Should_Ignore_It() {
        var eventType1      = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var now             = DateTimeOffset.UtcNow;
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: now);
        var firstOccurrence = TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: now.AddDays(1), isDeleted:true);
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: firstOccurrence.OccurredAt.AddDays(1));
        var result          = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, null));
        result.Should().NotBeNull();
        result!.DurationsCount.Should().Be(2);
        result.OccurrencesCount.Should().Be(0);
    }
    
    /// <summary>
    /// CASE 7: there is no durations, but some occurrences
    /// </summary>
    [Fact]
    public async Task Given_Occurrence_But_No_Duration_Should_Return_Occurrences() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        TestData.Events.CreateOccurrenceEvent(eventType1.CreatedById, eventType1.Id, occurredAt: DateTimeOffset.UtcNow);
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, null));
        result.Should().NotBeNull();
        result!.DurationsCount.Should().Be(0);
        result.OccurrencesCount.Should().Be(1);
    }
    
    /// <summary>
    /// CASE 8: there is no occurrences but some durations
    /// </summary>
    [Fact]
    public async Task Given_Durations_But_No_Occurrences_Should_Return_Occurrences() {
        var eventType1 = TestData.Events.CreateEventType(Guid.NewGuid(), "Test Event Type #1");
        var now        = DateTimeOffset.UtcNow;
        TestData.Events.CreateDurationEvent(eventType1.CreatedById, eventType1.Id, startedAt: now);
        var result = await Mediator.Send(new CalculateCountForEventType(eventType1.Id, null));
        result.Should().NotBeNull();
        result!.DurationsCount.Should().Be(1);
        result.OccurrencesCount.Should().Be(0);
    }
}