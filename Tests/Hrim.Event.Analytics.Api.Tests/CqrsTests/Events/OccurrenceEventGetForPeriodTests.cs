using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class OccurrenceEventGetForPeriodTests : BaseCqrsTests
{
    private readonly UserEventType _eventType;

    public OccurrenceEventGetForPeriodTests()
    {
        _eventType = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");
    }

    [Fact]
    public async Task Should_Return_Only_Mine_Events()
    {
        var start = DateTime.Now.Date.AddDays(-1).ToDateOnly();
        var end = start.AddDays(2);
        var mineEvent = TestData.Events.CreateOccurrenceEvent(OperatorContext.UserId,
            _eventType.Id,
            occurredAt: DateTimeOffset.Now);
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        TestData.Events.CreateOccurrenceEvent(anotherUserId,
            _eventType.Id,
            occurredAt: DateTimeOffset.Now);

        var query = new OccurrenceEventGetForPeriod(start, end, OperatorContext);
        var resultList = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(1);
        resultList.First().Id.Should().Be(mineEvent.Id);
    }

    [Fact]
    public async Task Should_Return_Within_Given_Period()
    {
        var start = DateTime.Now.Date.AddDays(-1).ToDateOnly();
        var end = start.AddDays(2);
        TestData.Events.CreateManyOccurrenceEvents(3, OperatorContext.UserId,
            start, end, _eventType.Id);

        var query = new OccurrenceEventGetForPeriod(start, end, OperatorContext);
        var resultList = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(3);
        resultList.All(x => x.OccurredAt >= start.CombineWithTime(TestUtils.DayStart)).Should().BeTrue();
    }
}