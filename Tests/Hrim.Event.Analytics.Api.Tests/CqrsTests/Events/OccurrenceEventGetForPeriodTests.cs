using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class OccurrenceEventGetForPeriodTests: BaseCqrsTests
{
    private readonly UserEventType _eventType;

    public OccurrenceEventGetForPeriodTests() { _eventType = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}"); }

    [Fact]
    public async Task Should_Return_Only_Mine_Events() {
        var start = DateTime.Now.Date.AddDays(value: -1).ToDateOnly();
        var end   = start.AddDays(value: 2);
        var mineEvent = TestData.Events.CreateOccurrenceEvent(userId: OperatorUserId,
                                                              eventTypeId: _eventType.Id,
                                                              occurredAt: DateTimeOffset.Now);
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(id: anotherUserId);
        TestData.Events.CreateOccurrenceEvent(userId: anotherUserId,
                                              eventTypeId: _eventType.Id,
                                              occurredAt: DateTimeOffset.Now);

        var query      = new OccurrenceEventGetForPeriod(Start: start, End: end, Context: OperatorContext);
        var resultList = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 1);
        resultList.First().Id.Should().Be(expected: mineEvent.Id);
    }

    [Fact]
    public async Task Should_Return_Within_Given_Period() {
        var start = DateTime.Now.Date.AddDays(value: -1).ToDateOnly();
        var end   = start.AddDays(value: 2);
        TestData.Events.CreateManyOccurrenceEvents(count: 3,
                                                   userId: OperatorUserId,
                                                   start: start,
                                                   end: end,
                                                   eventTypeId: _eventType.Id);

        var query      = new OccurrenceEventGetForPeriod(Start: start, End: end, Context: OperatorContext);
        var resultList = await Mediator.Send(request: query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(expected: 3);
        resultList.All(x => x.OccurredAt >= start.CombineWithTime(time: TestUtils.DayStart)).Should().BeTrue();
    }
}