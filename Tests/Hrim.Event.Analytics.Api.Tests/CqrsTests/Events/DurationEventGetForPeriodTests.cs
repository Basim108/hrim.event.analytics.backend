using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class DurationEventGetForPeriodTests: BaseCqrsTests
{
    private readonly EventType _eventType;

    public DurationEventGetForPeriodTests() { _eventType = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}").Bl; }

    [Fact]
    public async Task Should_Return_Only_Mine_Events() {
        var start = DateTime.Now.Date.AddDays(value: -1).ToDateOnly();
        var end   = start.AddDays(value: 2);
        var mineEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId,
                                                            eventTypeId: _eventType.Id,
                                                            startedAt: DateTimeOffset.Now,
                                                            finishedAt: DateTimeOffset.Now);
        var anotherUserId = new Random().NextInt64();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        TestData.Events.CreateDurationEvent(userId: anotherUserId,
                                            eventTypeId: _eventType.Id,
                                            startedAt: DateTimeOffset.Now,
                                            finishedAt: DateTimeOffset.Now);

        var query      = new DurationEventGetForPeriod(Start: start, End: end, Context: OperatorContext);
        var resultList = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 1);
        resultList.First().Id.Should().Be(expected: mineEvent.Id);
    }

    [Fact]
    public async Task Should_Return_Within_Given_Period() {
        var start = DateTime.Now.Date.AddDays(value: -1).ToDateOnly();
        var end   = start.AddDays(value: 2);
        TestData.Events.CreateManyDurationEvents(count: 3,
                                                 userId: OperatorUserId,
                                                 start: start,
                                                 end: end,
                                                 eventTypeId: _eventType.Id);

        var query      = new DurationEventGetForPeriod(Start: start, End: end, Context: OperatorContext);
        var resultList = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 3);
        resultList.All(x => x.StartedAt  >= start.CombineWithTime(time: TestUtils.DayStart)).Should().BeTrue();
        resultList.All(x => x.FinishedAt <= end.CombineWithTime(time: TestUtils.DayEnd)).Should().BeTrue();
    }

    [Fact]
    public async Task Should_Include_Events_Crossed_Start_Border() {
        var start = DateTimeOffset.UtcNow.Date.AddDays(value: -1);
        var end   = start.AddDays(value: 2);
        var createdEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId,
                                                               eventTypeId: _eventType.Id,
                                                               isDeleted: false,
                                                               startedAt: start,
                                                               finishedAt: end);
        var reportPeriodStart = start.ToDateOnly().AddDays(value: 1);
        var reportPeriodEnd   = reportPeriodStart.AddDays(value: 1);
        var query             = new DurationEventGetForPeriod(Start: reportPeriodStart, End: reportPeriodEnd, Context: OperatorContext);
        var resultList        = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.FirstOrDefault(x => x.Id == createdEvent.Id)
                  .Should()
                  .NotBeNull();
    }

    [Fact]
    public async Task Should_Include_Events_Crossed_End_Border() {
        var start = DateTime.UtcNow;
        var end   = start.AddDays(value: 2);
        var createdEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId,
                                                               eventTypeId: _eventType.Id,
                                                               isDeleted: false,
                                                               startedAt: start,
                                                               finishedAt: end);
        // reportStart should be < eventStart 
        // reportEnd should be > eventEnd
        var reportPeriodStart = start.ToDateOnly().AddDays(value: -1);
        var reportPeriodEnd   = end.ToDateOnly().AddDays(value: 1);
        var query             = new DurationEventGetForPeriod(Start: reportPeriodStart, End: reportPeriodEnd, Context: OperatorContext);
        var resultList        = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.FirstOrDefault(x => x.Id == createdEvent.Id)
                  .Should()
                  .NotBeNull();
    }

    [Fact]
    public async Task Should_Include_Events_That_Completely_Includes_Report_Period() {
        var start = DateTimeOffset.UtcNow.Date.AddDays(value: -1);
        var end   = start.AddDays(value: 3);
        var createdEvent = TestData.Events.CreateDurationEvent(userId: OperatorUserId,
                                                               eventTypeId: _eventType.Id,
                                                               isDeleted: false,
                                                               startedAt: start,
                                                               finishedAt: end);
        var reportPeriodStart = start.ToDateOnly().AddDays(value: 1);
        var reportPeriodEnd   = start.ToDateOnly().AddDays(value: 2);
        var query             = new DurationEventGetForPeriod(Start: reportPeriodStart, End: reportPeriodEnd, Context: OperatorContext);
        var resultList        = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.FirstOrDefault(x => x.Id == createdEvent.Id)
                  .Should()
                  .NotBeNull();
    }
}