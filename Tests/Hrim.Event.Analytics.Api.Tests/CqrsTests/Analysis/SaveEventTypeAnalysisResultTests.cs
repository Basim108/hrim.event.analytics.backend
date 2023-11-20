using FluentAssertions;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Analysis.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Analysis;

public class SaveEventTypeAnalysisResultTests: BaseCqrsTests
{
    /// <summary>
    /// CASE 01: Given null loadedDbEntity should create a new one 
    /// </summary>
    [Fact]
    public async Task Given_Null_LoadedDbEntity_Should_Create_A_New_One() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type");
        await Mediator.Send(new SaveEventTypeAnalysisResult(LoadedDbEntity: null,
                                                            EventTypeId: eventType.Id,
                                                            AnalysisCode: FeatureCodes.GAP_ANALYSIS,
                                                            ResultJson: null,
                                                            StartedAt: DateTime.UtcNow,
                                                            FinishedAt: DateTime.UtcNow,
                                                            CorrelationId: Guid.NewGuid()));
        var list = await TestData.DbContext.StatisticsForEventTypes.ToListAsync();
        list.Count.Should().Be(1);
        list[0].EntityId.Should().Be(eventType.Id);
    }
    
    /// <summary>
    /// CASE 02: Given LoadedDbEntity should not create a new and use this one
    /// </summary>
    [Fact]
    public async Task Given_LoadedDbEntity_Should_Not_Create_A_New_One() {
        var eventType = TestData.Events.CreateEventType(new Random().NextInt64(), "Test Event Type");
        var lastRun = new StatisticsForEventType() {
            EntityId = eventType.Id,
            AnalysisCode = FeatureCodes.GAP_ANALYSIS,
            ResultJson = null,
            StartedAt = DateTime.UtcNow.AddDays(-1),
            FinishedAt = DateTime.UtcNow.AddDays(-1),
            CorrelationId = Guid.NewGuid()
        };
        TestData.DbContext.StatisticsForEventTypes.Add(lastRun);
        TestData.DbContext.SaveChanges();
        await Mediator.Send(new SaveEventTypeAnalysisResult(LoadedDbEntity: lastRun,
                                                            EventTypeId: eventType.Id,
                                                            AnalysisCode: FeatureCodes.GAP_ANALYSIS,
                                                            ResultJson: "new one",
                                                            StartedAt: DateTime.UtcNow,
                                                            FinishedAt: DateTime.UtcNow,
                                                            CorrelationId: Guid.NewGuid()));
        var list = await TestData.DbContext.StatisticsForEventTypes.ToListAsync();
        list.Count.Should().Be(1);
        list[0].EntityId.Should().Be(eventType.Id);
        list[0].ResultJson.Should().Be("new one");
    }
}