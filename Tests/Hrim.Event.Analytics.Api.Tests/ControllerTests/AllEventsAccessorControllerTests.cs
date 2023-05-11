using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Tests.CqrsTests;
using Hrim.Event.Analytics.Api.V1.Controllers;
using Hrim.Event.Analytics.Api.V1.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Hrim.Event.Analytics.Api.Tests.ControllerTests;

[ExcludeFromCodeCoverage]
public class AllEventsAccessorControllerTests: BaseCqrsTests
{
    private readonly AllEventsAccessorController _controller;
    private readonly UserEventType               _eventType;
    private readonly ByPeriodRequest             _request;

    public AllEventsAccessorControllerTests() {
        var accessor = ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        _controller = new AllEventsAccessorController(requestAccessor: accessor, mediator: Mediator);

        _eventType = TestData.Events.CreateEventType(userId: OperatorUserId, $"Headache-{Guid.NewGuid()}");
        _request = new ByPeriodRequest {
            Start = DateTime.Now.Date.AddDays(value: -1).ToDateOnly(),
            End   = DateTime.Now.Date.AddDays(value: 1).ToDateOnly()
        };
    }

    [Fact]
    public async Task Having_No_Occurrence_Should_Returns_EmptyList() {
        TestData.Events.CreateManyDurationEvents(count: 3,
                                                 userId: OperatorUserId,
                                                 start: _request.Start,
                                                 end: _request.End,
                                                 eventTypeId: _eventType.Id);
        var result = await _controller.GetUserEventsAsync(request: _request, cancellationToken: CancellationToken.None);

        result.Should().NotBeNull();
        result.Durations.Should().NotBeEmpty();
        result.Occurrences.Should().NotBeNull();
        result.Occurrences.Should().BeEmpty();
    }

    [Fact]
    public async Task Having_No_Duration_Should_Returns_EmptyList() {
        TestData.Events.CreateManyOccurrenceEvents(count: 3,
                                                   userId: OperatorUserId,
                                                   start: _request.Start,
                                                   end: _request.End,
                                                   eventTypeId: _eventType.Id);
        var result = await _controller.GetUserEventsAsync(request: _request, cancellationToken: CancellationToken.None);

        result.Should().NotBeNull();
        result.Occurrences.Should().NotBeEmpty();
        result.Durations.Should().NotBeNull();
        result.Durations.Should().BeEmpty();
    }

    [Fact]
    public async Task Having_Durations_And_Occurrence_Should_Return_Correct() {
        TestData.Events.CreateManyDurationEvents(count: 3,
                                                 userId: OperatorUserId,
                                                 start: _request.Start,
                                                 end: _request.End,
                                                 eventTypeId: _eventType.Id);
        TestData.Events.CreateManyOccurrenceEvents(count: 3,
                                                   userId: OperatorUserId,
                                                   start: _request.Start,
                                                   end: _request.End,
                                                   eventTypeId: _eventType.Id);
        var result = await _controller.GetUserEventsAsync(request: _request, cancellationToken: CancellationToken.None);

        result.Should().NotBeNull();
        result.Occurrences.Should().NotBeEmpty();
        result.Durations.Should().NotBeEmpty();
        result.Request.Should().NotBeNull();
        result.Request.Start.Should().Be(expected: _request.Start);
        result.Request.End.Should().Be(expected: _request.End);
    }

    [Fact]
    public async Task Having_No_Events_Should_Returns_RequestInfo() {
        var result = await _controller.GetUserEventsAsync(request: _request, cancellationToken: CancellationToken.None);

        result.Should().NotBeNull();
        result.Request.Should().NotBeNull();
        result.Request.Start.Should().Be(expected: _request.Start);
        result.Request.End.Should().Be(expected: _request.End);
    }
}