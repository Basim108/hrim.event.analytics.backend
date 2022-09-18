using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.V1.Controllers;
using Hrim.Event.Analytics.Api.V1.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Events;

public class AllEventsAccessorControllerTests: BaseCqrsTests {
    private readonly AllEventsAccessorController _controller;
    private readonly ByPeriodRequest             _request;
    private readonly UserEventType               _eventType;

    public AllEventsAccessorControllerTests() {
        var accessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        _controller = new AllEventsAccessorController(accessor, Mediator);
        
        _eventType  = TestData.Events.CreateEventType(OperatorContext.UserId, $"Headache-{Guid.NewGuid()}");
        _request = new ByPeriodRequest {
            Start = DateTime.Now.Date.AddDays(-1).ToDateOnly(),
            End   = DateTime.Now.Date.AddDays(1).ToDateOnly()
        };
    }

    [Fact]
    public async Task Having_No_Occurrence_Should_Returns_EmptyList() {
        TestData.Events.CreateManyDurationEvents(count: 3, OperatorContext.UserId,
                                                 _request.Start, _request.End, _eventType.Id);
        var result = await _controller.GetUserEventsAsync(_request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Durations.Should().NotBeEmpty();
        result.Occurrences.Should().NotBeNull();
        result.Occurrences.Should().BeEmpty();
    }

    [Fact]
    public async Task Having_No_Duration_Should_Returns_EmptyList() {
        TestData.Events.CreateManyOccurrenceEvents(count: 3, OperatorContext.UserId,
                                                   _request.Start, _request.End, _eventType.Id);
        var result = await _controller.GetUserEventsAsync(_request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Occurrences.Should().NotBeEmpty();
        result.Durations.Should().NotBeNull();
        result.Durations.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Having_Durations_And_Occurrence_Should_Return_Correct() {
        TestData.Events.CreateManyDurationEvents(count: 3, OperatorContext.UserId,
                                                   _request.Start, _request.End, _eventType.Id);
        TestData.Events.CreateManyOccurrenceEvents(count: 3, OperatorContext.UserId,
                                                   _request.Start, _request.End, _eventType.Id);
        var result = await _controller.GetUserEventsAsync(_request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Occurrences.Should().NotBeEmpty();
        result.Durations.Should().NotBeEmpty();
        result.Request.Should().NotBeNull();
        result.Request.Start.Should().Be(_request.Start);
        result.Request.End.Should().Be(_request.End);
    }

    [Fact]
    public async Task Having_No_Events_Should_Returns_RequestInfo() {
        var result = await _controller.GetUserEventsAsync(_request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Request.Should().NotBeNull();
        result.Request.Start.Should().Be(_request.Start);
        result.Request.End.Should().Be(_request.End);
    }
}