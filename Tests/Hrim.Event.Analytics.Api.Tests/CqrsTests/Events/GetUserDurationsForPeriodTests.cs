using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

[ExcludeFromCodeCoverage]
public class GetUserDurationsForPeriodTests: BaseCqrsTests {
    private readonly UserEventType _eventType;

    public GetUserDurationsForPeriodTests() {
        _eventType = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");
    }

    [Fact]
    public async Task Should_Return_Only_Mine_Events() {
        var start = DateTime.Now.Date.AddDays(-1).ToDateOnly();
        var end   = start.AddDays(2);
        var mineEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId,
                                                            _eventType.Id,
                                                            startedAt: DateTimeOffset.Now,
                                                            finishedAt: DateTimeOffset.Now);
        var anotherUserId = Guid.NewGuid();
        TestData.Users.EnsureUserExistence(anotherUserId);
        TestData.Events.CreateDurationEvent(anotherUserId,
                                            _eventType.Id,
                                            startedAt: DateTimeOffset.Now,
                                            finishedAt: DateTimeOffset.Now);

        var query      = new GetUserDurationsForPeriod(start, end, OperatorContext);
        var resultList = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(1);
        resultList.First().Id.Should().Be(mineEvent.Id);
    }

    [Fact]
    public async Task Should_Return_Within_Given_Period() {
        var start = DateTime.Now.Date.AddDays(-1).ToDateOnly();
        var end   = start.AddDays(2);
        TestData.Events.CreateManyDurationEvents(count: 3, OperatorContext.UserId,
                                                 start, end, _eventType.Id);

        var query      = new GetUserDurationsForPeriod(start, end, OperatorContext);
        var resultList = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.Count.Should().Be(3);
        resultList.All(x => x.StartedAt  >= start.CombineWithTime(TestUtils.DayStart)).Should().BeTrue();
        resultList.All(x => x.FinishedAt <= end.CombineWithTime(TestUtils.DayEnd)).Should().BeTrue();
    }

    [Fact]
    public async Task Should_Include_Events_Crossed_Start_Border() {
        var start = DateTimeOffset.UtcNow.Date.AddDays(-1);
        var end   = start.AddDays(2);
        var createdEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, _eventType.Id,
                                                               isDeleted: false, start, end);
        var reportPeriodStart = start.ToDateOnly().AddDays(1);
        var reportPeriodEnd   = reportPeriodStart.AddDays(1);
        var query             = new GetUserDurationsForPeriod(reportPeriodStart, reportPeriodEnd, OperatorContext);
        var resultList        = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.FirstOrDefault(x => x.Id == createdEvent.Id)
                  .Should().NotBeNull();
    }
    
    [Fact]
    public async Task Should_Include_Events_Crossed_End_Border() {
        var start = DateTime.UtcNow;
        var end   = start.AddDays(2);
        var createdEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, _eventType.Id,
                                                               isDeleted: false, start, end);
        // reportStart should be < eventStart 
        // reportEnd should be > eventEnd
        var reportPeriodStart = start.ToDateOnly().AddDays(-1);
        var reportPeriodEnd   = end.ToDateOnly().AddDays(1);
        var query             = new GetUserDurationsForPeriod(reportPeriodStart, reportPeriodEnd, OperatorContext);
        var resultList        = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.FirstOrDefault(x => x.Id == createdEvent.Id)
                  .Should().NotBeNull();
    }
    
    [Fact]
    public async Task Should_Include_Events_That_Completely_Includes_Report_Period() {
        var start = DateTimeOffset.UtcNow.Date.AddDays(-1);
        var end   = start.AddDays(3);
        var createdEvent = TestData.Events.CreateDurationEvent(OperatorContext.UserId, _eventType.Id,
                                                               isDeleted: false, start, end);
        var reportPeriodStart = start.ToDateOnly().AddDays(1);
        var reportPeriodEnd   = start.ToDateOnly().AddDays(2);
        var query             = new GetUserDurationsForPeriod(reportPeriodStart, reportPeriodEnd, OperatorContext);
        var resultList        = await Mediator.Send(query);

        resultList.Should().NotBeEmpty();
        resultList.FirstOrDefault(x => x.Id == createdEvent.Id)
                  .Should().NotBeNull();
    }
}